using System;
using Microsoft.Extensions.Options;
using Nest;

namespace Shared.Elasticsearch
{
    public class ElasticClientProvider<TElasticConnectionSetting>
        where TElasticConnectionSetting : ElasticConnectionSetting, new()
    {
        public ElasticClientProvider(IOptions<TElasticConnectionSetting> setting)
        {
            var connectionSettings = new ConnectionSettings(new Uri(setting.Value.ClusterUrl));

            connectionSettings.EnableDebugMode();

            if (setting.Value.Index != null) connectionSettings.DefaultIndex(setting.Value.Index);

            Client = new ElasticClient(connectionSettings);
        }

        public ElasticClient Client { get; }
    }
}