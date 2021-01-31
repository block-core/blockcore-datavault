using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Models
{
    public class EntitySerializeBase
    {
        public string Id { get; set; }

        public string Content { get; }
    }

    [BsonKnownTypes(typeof(Entity))]
    public class Entity : DynamicObject
    {
        // [BsonId]
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        [BsonElement("content")]
        public BsonDocument Content { get; set; }
    }

    public class EntityRequest
    {
        [BsonId]
        public string Id { get; set; }

        public string Content { get; set; }
    }
}
