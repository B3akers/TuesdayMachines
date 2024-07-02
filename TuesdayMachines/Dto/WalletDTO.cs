using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TuesdayMachines.Dto
{
    public class WalletDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BroadcasterAccountId { get; set; }

        public string TwitchUserId { get; set; }

        public long Balance { get; set; }
    }
}
