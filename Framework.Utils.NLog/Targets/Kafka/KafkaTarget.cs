namespace Framework.Utils.NLog.Targets.Kafka
{
    using global::NLog.Common;
    using global::NLog.Targets;
    using KafkaNet.Protocol;
    using Utils.Kafka;

    [Target("Kafka")]
    public class KafkaTarget : TargetWithLayout
    {
        /// <summary>
        /// https://github.com/nlog/nlog/wiki/Extending%20NLog
        /// </summary>
        /// <param name="logEvent"></param>
        protected override async void Write(AsyncLogEventInfo logEvent)
        {
            var formattedMessage = this.Layout.Render(logEvent.LogEvent);

            await KafkaBootstrapper.Instance.Client.SendMessageAsync(logEvent.LogEvent.LoggerName,
                    new[] { new Message(formattedMessage) });
        }
    }
}
