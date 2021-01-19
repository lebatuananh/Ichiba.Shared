using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Shared.MongoDb.Config;

namespace Ez.OrderServices.ReadSide.MQ.Domain
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; }
    }

    public class MongoDbContext : IMongoDbContext
    {
        public IMongoDatabase Database { get; }

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                Database = client.GetDatabase(settings.Value.Database);
        }
    }
}