﻿namespace Framework.Utils.NLog.Targets.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Elasticsearch.Net;
    using global::NLog.Common;
    using global::NLog.Config;
    using global::NLog.Layouts;
    using global::NLog.Targets;

    [Target("ElasticSearch")]
    public class ElasticSearchTarget : TargetWithLayout
    {
        private IElasticLowLevelClient client;

        private List<string> excludedProperties =
            new List<string>(new[] {"CallerMemberName", "CallerFilePath", "CallerLineNumber", "MachineName", "ThreadId"});

        public string Uri { get; set; }
        public Layout Index { get; set; }
        public bool IncludeAllProperties { get; set; }
        public string ExcludedProperties { get; set; }

        [RequiredParameter]
        public Layout DocumentType { get; set; }

        /// <summary>
        /// Gets or sets a list of additional fields to add to the elasticsearch document.
        /// </summary>
        [ArrayParameter(typeof(ElasticField), "field")]
        public IList<ElasticField> Fields { get; set; }

        public IElasticsearchSerializer ElasticsearchSerializer { get; set; }

        /// <summary>
        /// Gets or sets if exceptions will be rethrown.
        /// 
        /// Set it to true if ElasticSearchTarget target is used within FallbackGroup target (https://github.com/NLog/NLog/wiki/FallbackGroup-target).
        /// </summary>
        public bool ThrowExceptions { get; set; }

        public ElasticSearchTarget()
        {
            Name = "ElasticSearch";
            Uri = "http://localhost:9200";
            DocumentType = "logevent";
            Index = "logstash-${date:format=yyyy.MM.dd}";
            Fields = new List<ElasticField>();
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            var uri = Uri;
            var nodes = uri.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(url => new Uri(url));
            var connectionPool = new StaticConnectionPool(nodes);
            IConnectionConfigurationValues config = new ConnectionConfiguration(connectionPool);
            if (ElasticsearchSerializer != null)
                config = new ConnectionConfiguration(connectionPool, _ => ElasticsearchSerializer);
            this.client = new ElasticLowLevelClient(config);

            if (!string.IsNullOrEmpty(ExcludedProperties))
                this.excludedProperties =
                    ExcludedProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(new[] {logEvent});
        }

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            SendBatch(logEvents);
        }

        private void SendBatch(IEnumerable<AsyncLogEventInfo> events)
        {
            try
            {
                var logEvents = events.Select(e => e.LogEvent);
                var payload = new List<object>();

                foreach (var logEvent in logEvents)
                {
                    var document = new Dictionary<string, object>
                    {
                        {"@timestamp", logEvent.TimeStamp},
                        {"level", logEvent.Level.Name},
                        {"message", Layout.Render(logEvent)}
                    };

                    if (logEvent.Exception != null)
                        document.Add("exception", logEvent.Exception);

                    foreach (var field in Fields)
                    {
                        var renderedField = field.Layout.Render(logEvent);
                        if (!string.IsNullOrWhiteSpace(renderedField))
                            document[field.Name] = renderedField.ToSystemType(field.LayoutType);
                    }

                    if (IncludeAllProperties)
                    {
                        foreach (
                            var p in logEvent.Properties.Where(p => !this.excludedProperties.Contains(p.Key.ToString()))
                                .Where(p => !document.ContainsKey(p.Key.ToString())))
                        {
                            document[p.Key.ToString()] = p.Value;
                        }
                    }

                    var index = Index.Render(logEvent).ToLowerInvariant();
                    var type = DocumentType.Render(logEvent);

                    payload.Add(new {index = new {_index = index, _type = type}});
                    payload.Add(document);
                }

                var result = this.client.Bulk<byte[]>(payload);

                if (!result.Success)
                    InternalLogger.Error("Failed to send log messages to elasticsearch: status={0}, message=\"{1}\"",
                        result.HttpStatusCode,
                        result.OriginalException?.Message ??
                        "No error message. Enable Trace logging for more information.");
            }
            catch (Exception ex)
            {
                InternalLogger.Error("Error while sending log messages to elasticsearch: message=\"{0}\"", ex.Message);
            }
        }
    }
}


