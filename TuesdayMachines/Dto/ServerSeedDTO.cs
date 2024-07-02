using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TuesdayMachines.Dto
{
    public class ServerSeedDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string HashedKey { get; set; }
        public string Key { get; set; }
        public long CreationTime { get; set; }
    }
}
