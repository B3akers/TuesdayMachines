using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TuesdayMachines.Dto
{
    public class SpinDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string AccountId { get; set; }
        public string Seed { get; set; }
        public string Game { get; set; }
        public string Wallet { get; set; }
        public long Bet { get; set; }
        public long Result { get; set; }
        public long Datetime { get; set; }
    }
}
