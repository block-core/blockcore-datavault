using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Storage
{
    public interface IStorageRepository
    {
        Task<IEnumerable<dynamic>> Get(string collection, string filter);

        Task<dynamic> GetById(string collection, string id);

        Task Create(string collection, BsonDocument entity);

        Task<bool> Update(string collection, string id, BsonDocument entity);

        Task<bool> Delete(string collection, string id);
    }
}
