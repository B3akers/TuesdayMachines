using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TuesdayMachines.Dto
{
    public class UserSeedDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string AccountId { get; set; }
        public string Client { get; set; }
        public string ServerSeed { get; set; }
        public string NextServerSeed { get; set; }
        public long Nonce { get; set; }
    }
}
