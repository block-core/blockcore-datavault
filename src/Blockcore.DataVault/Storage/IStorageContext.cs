using Blockcore.DataVault.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Storage
{
    public interface IStorageContext
    {
        IMongoCollection<Entity> Entities { get; }

        IMongoCollection<T> Collection<T>(string collection);
    }
}
