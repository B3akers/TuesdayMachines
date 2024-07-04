using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TuesdayMachines.Dto
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(MinesActiveGameDTO), typeof(RouletteActiveSeedDTO))]
    public class ActiveGameDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Key { get; set; }
    }

    public class MinesActiveGameDTO : ActiveGameDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string AccountId { get; set; }
        public long Bet { get; set; }
        public int[] Picked { get; set; }
        public int[] Bombs { get; set; }
    }

    public class RouletteActiveSeedDTO : ActiveGameDTO
    {
        public string ClientSeed { get; set; }
        public string ServerSeed { get; set; }
        public string NextServerSeed { get; set; }
        public long Nonce { get; set; }
        public long CreateTime { get; set; }
    }

}
