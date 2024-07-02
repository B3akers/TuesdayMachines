using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TuesdayMachines.Dto
{
    public class BroadcasterDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string AccountId { get; set; }
        public string TwitchId { get; set; }
        public string Login { get; set; }
        public string Points { get; set; }
        public long WatchPoints { get; set; }
        public long WatchPointsSub { get; set; }
    }
}
