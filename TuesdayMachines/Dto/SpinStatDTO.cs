using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace TuesdayMachines.Dto
{
    public class SpinStatDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string AccountId { get; set; }
        public string Game { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string Wallet { get; set; }
        [JsonIgnore]
        public string Seed { get; set; }
        public long Bet { get; set; }
        public long Win { get; set; }
        public long WinX { get; set; }
        public long Datetime { get; set; }
    }
}
