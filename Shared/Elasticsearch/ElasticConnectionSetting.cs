namespace Shared.Elasticsearch
{
    public class ElasticConnectionSetting
    {
        private string index;
        public string ClusterUrl { get; set; }

        public string Index
        {
            get => index;
            set => index = value.ToLower();
        }
    }
}