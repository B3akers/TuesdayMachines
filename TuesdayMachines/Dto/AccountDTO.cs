using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TuesdayMachines.Dto
{
    public class AccountDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TwitchId { get; set; }
        public string TwitchLogin { get; set; }
        public string EncAccessToken { get; set; }
        public string EncRefreshToken { get; set; }
        public int AccountType { get; set; }
        public long AuthTimestamp { get; set; }
        public long CreationTime { get; set; }

        public bool IsAdmin()
        {
            return (AccountType & (1 << 0)) != 0;
        }

        public bool IsBroadcaster()
        {
            return (AccountType & (1 << 1)) != 0;
        }
    }
}
