using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Storage
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IStorageContext context;

        public StorageRepository(IStorageContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<dynamic>> Get(string collection, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return await context.Collection<dynamic>(collection).Find(_ => true).ToListAsync();
            }
            else
            {
                var values = filter.Split(':');
                var filterDocument = new BsonDocument { { values[0], values[1] } };
                return await context.Collection<dynamic>(collection).Find(filterDocument).ToListAsync();
            }
        }

        public Task<dynamic> GetById(string collection, string id)
        {
            var objectId = new ObjectId(id);
            var filter = new BsonDocument { { "_id", objectId } };

            return context.Collection<dynamic>(collection).Find(filter).FirstOrDefaultAsync();

            //var objectId = new ObjectId(id);

            //context.Collection<BsonDocument>(collection).Find

            //var filter = Builders<dynamic>.Filter.Eq(new StringFieldDefinition<dynamic, string>("Id"), id);
            //// FilterDefinition<Entity> filter = Builders<dynamic>.Filter.Eq(m => m.Id, id);

            //return context
            //        .Collection<dynamic>(collection)
            //        .Find(filter)
            //        .FirstOrDefaultAsync();
        }

        public async Task Create(string collection, BsonDocument entity)
        {
            await context.Collection<BsonDocument>(collection).InsertOneAsync(entity);
        }

        public async Task<bool> Update(string collection, string id, BsonDocument entity)
        {
            // Example:
            // var sort = Builders<BsonDocument>.Sort.Descending($"content.date");

            // var projection = Builders<BsonDocument>.Projection.Include

            var filter = new BsonDocument { { "_id", new ObjectId(id) } };

            // var idDefinition = new StringFieldDefinition<BsonDocument, string>("_id");
            // var idFilter = Builders<BsonDocument>.Filter.Eq(idDefinition, objectId);
            // var books = context.Collection<dynamic>(collection).Find(idFilter).ToList();

            //var idFieldDefinition = new StringFieldDefinition<Entity, string>("_id");
            //var inStockFieldDefinition = new StringFieldDefinition<BsonDocument, bool>("InStock");


            // FilterDefinition inStockFilter = Builders.Filter.Eq(inStockFieldDefinition, true);


            //var filter = Builders<BsonDocument>.Filter.Where(s => s.Id_id == id && s.Price == 1500);

            //var filter = Builders<User>.Filter.Eq(x => x.A, "1");
            //filter = filter & (Builders<User>.Filter.Eq(x => x.B, "4") | Builders<User>.Filter.Eq(x => x.B, "5"));

            //ReplaceOneResult replaceOneResult = context.Collection<dynamic>(collection).ReplaceOne(Builders<dynamic>.Filter.Eq(.Eq(r => r.Id, id), entity, new UpdateOptions() { IsUpsert = true });

            //Console.WriteLine(replaceOneResult.IsAcknowledged);
            //Console.WriteLine(replaceOneResult.MatchedCount);

            //await context.Collection<dynamic>(collection).FindOneAndUpdate()


            //var collection = db.GetCollection<dynamic>("posts");
            //var data = collection.Find(Builders<dynamic>.Filter.Empty).ToList();
            //var firstMessage = data[0].Message; // dynamically typed code

            ReplaceOneResult updateResult =
                await context
                        .Collection<BsonDocument>(collection)
                        .ReplaceOneAsync(
                            filter: filter,
                            replacement: entity);
            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;

        }

        public async Task<bool> Delete(string collection, string id)
        {
            var filter = new BsonDocument { { "_id", new ObjectId(id) } };

            DeleteResult deleteResult = await context.Collection<dynamic>(collection).DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }
}
