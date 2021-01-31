using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Blockcore.DataVault.Models;
using Blockcore.DataVault.Settings;
using MongoDB.Driver;

namespace Blockcore.DataVault.Storage
{
    public class StorageContext : IStorageContext
    {
        private readonly IMongoDatabase db;

        public StorageContext(MongoDBConfig config)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(config.ConnectionString)
            );

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var client = new MongoClient(settings);

            db = client.GetDatabase(config.Database);
        }

        public IMongoCollection<T> Collection<T>(string collection)
        {
            return db.GetCollection<T>(collection);
        }

        public IMongoCollection<Entity> Entities => db.GetCollection<Entity>("Entities");
    }
}
