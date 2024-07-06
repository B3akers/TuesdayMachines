using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.Mvc;
using TuesdayMachines.Filters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Api
{
    public static class GameMinimalApi
    {
        public static void MapGameEndpoints(this IEndpointRouteBuilder app)
        {
            var games = app.MapGroup("game").AddEndpointFilter<HomeEndpointFilter>();

            games.MapGet("getgames", GetGames);
            games.MapGet("getwallets", GetWallets);
            games.MapGet("getcurrentseed", GetCurrentSeed);
            games.MapPost("getserverseed", GetServerSeed).AddEndpointFilter<ValidationFilter<GetServerSeedModel>>();
            games.MapPost("getwallet", GetWallet).AddEndpointFilter<ValidationFilter<IdModel>>();
            games.MapPost("play", Play).AddEndpointFilter<ValidationFilter<GamePlayModel>>();
            games.MapPost("getaccountsranking", GetAccountsRanking).AddEndpointFilter<ValidationFilter<IdModel>>();
            games.MapPost("changeseed", ChangeSeed).AddEndpointFilter<ValidationFilter<ChangeSeedModel>>();
            games.MapPost("getspinsstats", GetSpinsStats).AddEndpointFilter<ValidationFilter<GetSpinsStatsModel>>();
            games.MapPost("decryptserver", DecryptServer).AddEndpointFilter<ValidationFilter<DecryptServerModel>>();
        }

        public static async Task<IResult> GetSpinsStats([FromBody] GetSpinsStatsModel model, ISpinsRepository spinsRepository, IAccountsRepository accountsRepository, IBroadcastersRepository broadcastersRepository)
        {
            if (model.Time < DateTimeOffset.UtcNow.AddDays(-31).ToUnixTimeSeconds())
                return Results.Json(new { error = "invalid_model" });

            var result = await spinsRepository.GetSpinsStatsLogs(model.Time, model.Game, model.Wallet, 15, model.SortByXWin);
            var accounts = await accountsRepository.GetAccountsById(result.Select(x => x.AccountId));
            var broadcasters = string.IsNullOrEmpty(model.Wallet) ? await broadcastersRepository.GetBroadcasters(result.DistinctBy(x => x.Wallet).Select(x => x.Wallet)) : new List<Dto.BroadcasterDTO>() { await broadcastersRepository.GetBroadcasterByAccountId(model.Wallet) };

            return Results.Json(new
            {
                data = result,
                accounts = accounts.Select(x => new { x.Id, x.TwitchLogin }),
                wallets = broadcasters.Select(x => new { id = x.AccountId, x.Login, x.Points })
            });
        }

        public static async Task<IResult> GetCurrentSeed(IUserFairPlay userFairPlay, HttpContext context)
        {
            var account = context.Items["userAccount"] as Dto.AccountDTO;
            var seed = await userFairPlay.GetCurrentUserSeedInfo(account.Id);

            return Results.Json(new
            {
                seed.Nonce,
                seed.Client,
                server = seed.ServerSeed.HashSHA256(),
                nextServer = seed.NextServerSeed.HashSHA256()
            });
        }

        public static async Task<IResult> DecryptServer([FromBody] DecryptServerModel modal, IUserFairPlay userFairPlay)
        {
            var seed = await userFairPlay.GetDecryptedServerSeed(modal.ServerSeed);
            if (string.IsNullOrEmpty(seed))
                return Results.Json(new { error = "invalid_server_seed" });

            return Results.Json(new
            {
                seed
            });
        }

        public static async Task<IResult> ChangeSeed([FromBody] ChangeSeedModel model, IUserFairPlay userFairPlay, HttpContext context)
        {
            var account = context.Items["userAccount"] as Dto.AccountDTO;
            var newHash = await userFairPlay.ChangeUserSeed(account.Id, model.ClientSeed);

            return Results.Json(new
            {
                nonce = 0,
                client = model.ClientSeed,
                server = newHash.Item1.HashSHA256(),
                nextServer = newHash.Item2.HashSHA256()
            });
        }

        public static async Task<IResult> GetAccountsRanking([FromBody] IdModel model, IPointsRepository pointsRepository, IBroadcastersRepository broadcastersRepository, IAccountsRepository accountsRepository)
        {
            var broadcaster = await broadcastersRepository.GetBroadcasterByAccountId(model.Id);
            if (broadcaster == null)
                return Results.Json(new { error = "invalid_model" });

            var result = await pointsRepository.GetTopAccounts(broadcaster.AccountId, 100);
            var accounts = await accountsRepository.GetAccountsByTwitchId(result.Select(x => x.TwitchUserId));

            return Results.Json(new
            {
                data = result,
                accounts = accounts.Select(x => new { id = x.TwitchId, x.TwitchLogin }),
                wallet = new { id = broadcaster.AccountId, broadcaster.Login, broadcaster.Points }
            });
        }

        public static IResult Play([FromBody] GamePlayModel model)
        {
            if (model.GameId == "mayan")
                return Results.Json(new { redirect = $"/mayan?wallet={Uri.EscapeDataString(model.Wallet)}" });
            else if (model.GameId == "plinko")
                return Results.Json(new { redirect = $"/plinko?wallet={Uri.EscapeDataString(model.Wallet)}" });
            else if (model.GameId == "roulette")
                return Results.Json(new { redirect = $"/roulette?wallet={Uri.EscapeDataString(model.Wallet)}" });
            else if (model.GameId == "mines")
                return Results.Json(new { redirect = $"/mines?wallet={Uri.EscapeDataString(model.Wallet)}" });

            return Results.Json(new { error = "invalid_model" });
        }

        public static async Task<IResult> GetWallet([FromBody] IdModel id, IPointsRepository pointsRepository, IBroadcastersRepository broadcastersRepository, HttpContext context)
        {
            var broadcaster = await broadcastersRepository.GetBroadcasterByAccountId(id.Id);
            if (broadcaster == null)
                return Results.Json(new { error = "invalid_model" });

            var account = context.Items["userAccount"] as Dto.AccountDTO;
            var balance = await pointsRepository.GetBalance(account.TwitchId, broadcaster.AccountId);

            return Results.Json(new { balance = balance.Balance, name = broadcaster.Points });
        }

        public static async Task<IResult> GetServerSeed([FromBody] GetServerSeedModel model, IUserFairPlay userFairPlay)
        {
            if (model.Game == "roulette")
            {
                var seed = await userFairPlay.GetCurrentLiveRouletteRoundInfo();
                return Results.Json(new
                {
                    seed.Nonce,
                    client = seed.ClientSeed,
                    server = seed.ServerSeed.HashSHA256(),
                    nextServer = seed.NextServerSeed.HashSHA256()
                });
            }

            return Results.Json(new { error = "invalid_model" });
        }

        public static async Task<IResult> GetGames(IGamesRepository gamesRepository)
        {
            var games = await gamesRepository.GetGames();

            return Results.Json(new { data = games });
        }

        public static async Task<IResult> GetWallets(IPointsRepository pointsRepository, IBroadcastersRepository broadcastersRepository, HttpContext context)
        {
            var account = context.Items["userAccount"] as Dto.AccountDTO;
            var wallets = await pointsRepository.GetUserWallets(account.TwitchId);
            var broadcasters = await broadcastersRepository.GetBroadcasters(wallets.Select(x => x.BroadcasterAccountId));

            return Results.Json(new { wallets = wallets.Select(x => new { x.Balance, x.BroadcasterAccountId }), broadcasters = broadcasters.Select(x => new { x.AccountId, x.Login, x.Points }) });
        }
    }
}
