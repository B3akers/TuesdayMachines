using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TuesdayMachines.WebSockets;

namespace TuesdayMachines.Models
{
    [JsonDerivedType(typeof(MinesGamePlayModel), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(MinesGamePlayStartGameModel), typeDiscriminator: "start_game")]
    [JsonDerivedType(typeof(MinesGamePlayRevealTileModel), typeDiscriminator: "reveal_tile")]
    [JsonDerivedType(typeof(MinesGamePlayCashoutModel), typeDiscriminator: "cashout_game")]
    public class MinesGamePlayModel
    {

    }

    public class MinesGamePlayCashoutModel : MinesGamePlayModel
    {

    }

    public class MinesGamePlayRevealTileModel : MinesGamePlayModel
    {
        public int Index { get; set; }
    }

    public class MinesGamePlayStartGameModel : MinesGamePlayModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        [Required]
        public string Wallet { get; set; }
        public long Bet { get; set; }
        public int Mines { get; set; }
    }
}
