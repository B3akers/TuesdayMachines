using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TuesdayMachines.Filters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;

namespace TuesdayMachines.Api
{
    public static class PlinkoMinimalApi
    {
        private static readonly long[] _betsAllowedValues = [5, 10, 25, 50, 100, 150, 250, 500, 1000, 1500, 2000, 5000, 10000, 15000, 20000];
        public static void MapPlinkoEndpoints(this IEndpointRouteBuilder app)
        {
            var plinko = app.MapGroup("plinko").AddEndpointFilter<HomeEndpointFilter>();

            plinko.MapPost("play", Play)
                .RequireRateLimiting("spins")
                .AddEndpointFilter<ValidationFilter<DefaultGamePlayModel>>();
        }

        public static async Task<IResult> Play([FromBody] DefaultGamePlayModel model, IPointsRepository pointsRepository, IUserFairPlay userFairPlay, IPlinkoGame plinkoGame, ISpinsRepository spinsRepository, HttpContext context)
        {
            bool found = false;
            for (var i = 0; i < _betsAllowedValues.Length; i++)
            {
                if (model.Bet == _betsAllowedValues[i])
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return Results.Json(new { error = "invalid_model" });

            var account = context.Items["userAccount"] as Dto.AccountDTO;
            var result = pointsRepository.TakePoints(account.TwitchId, model.Wallet, model.Bet);
            if (!result.Success)
                return Results.Json(new { error = "not_sufficient_funds" });

            var roundInfo = userFairPlay.GetUserSeedRoundInfo(account.Id);
            var gameResult = plinkoGame.SimulateGame(roundInfo.Client, roundInfo.Server, roundInfo.Nonce, model.Bet);

            if (gameResult.TotalWin > 0)
            {
                result = pointsRepository.AddPoints(account.TwitchId, model.Wallet, gameResult.TotalWin);

                var X = (double)gameResult.TotalWin / gameResult.Bet;
                if (X >= 10.0)
                {
                    await spinsRepository.AddSpinStatLog(new Dto.SpinStatDTO()
                    {
                        AccountId = account.Id,
                        Bet = model.Bet,
                        Game = "plinko",
                        Wallet = model.Wallet,
                        Win = gameResult.TotalWin,
                        WinX = (long)X
                    });
                }
            }

            /*
            await spinsRepository.AddSpinLog(new Dto.SpinDTO()
            {
                AccountId = account.Id,
                Bet = model.Bet,
                Result = gameResult.TotalWin,
                Game = $"plinko_{_plinkoGame.GetVersion()}",
                Seed = $"{roundInfo.Client}:{roundInfo.Server}:{roundInfo.Nonce}",
                Wallet = model.Wallet
            });
            */

            gameResult.CurrentBalance = result.Balance;

            return Results.Json(gameResult);
        }
    }
}
