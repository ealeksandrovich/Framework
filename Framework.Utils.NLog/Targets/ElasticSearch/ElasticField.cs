namespace Framework.Utils.NLog.Targets.ElasticSearch
{
    using System;
    using global::NLog.Config;
    using global::NLog.Layouts;

    public class ElasticField
    {
        public ElasticField()
        {
            LayoutType = typeof(string);
        }

        [RequiredParameter]
        public string Name { get; set; }

        [RequiredParameter]
        public Layout Layout { get; set; }

        public Type LayoutType { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, LayoutType: {LayoutType}, Layout: {Layout}";
        }
    }
}
