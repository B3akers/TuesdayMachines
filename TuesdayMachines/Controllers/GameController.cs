using TuesdayMachines.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TuesdayMachines.ActionFilters;
using MongoDB.Driver;
using TuesdayMachines.Models;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(HomeActionFilter))]
    public class GameController : Controller
    {
        private readonly IPointsRepository _pointsRepository;
        private readonly IBroadcastersRepository _broadcastersRepository;
        private readonly IUserFairPlay _userFairPlay;
        private readonly IGamesRepository _gamesRepository;

        public GameController(IPointsRepository pointsRepository, IBroadcastersRepository broadcastersRepository, IUserFairPlay userFairPlay, IGamesRepository gamesRepository)
        {
            _pointsRepository = pointsRepository;
            _broadcastersRepository = broadcastersRepository;
            _userFairPlay = userFairPlay;
            _gamesRepository = gamesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetGames()
        {
            var games = await _gamesRepository.GetGames();

            return Json(new { data = await games.ToListAsync() });
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentSeed()
        {
            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            var seed = await _userFairPlay.GetCurrentUserSeedInfo(account.Id);

            return Json(new
            {
                seed.Nonce,
                seed.Client,
                server = seed.ServerSeed.HashSHA256(),
                nextServer = seed.NextServerSeed.HashSHA256()
            });
        }

        [HttpPost]
        public async Task<IActionResult> DecryptServer([FromBody] DecryptServerModel modal)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            var seed = await _userFairPlay.GetDecryptedServerSeed(modal.ServerSeed);
            if (string.IsNullOrWhiteSpace(seed))
                return Json(new { error = "invalid_server_seed" });

            return Json(new
            {
                seed
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeSeed([FromBody] ChangeSeedModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            var newHash = await _userFairPlay.ChangeUserSeed(account.Id, model.ClientSeed);

            return Json(new
            {
                nonce = 0,
                client = model.ClientSeed,
                server = newHash.Item1.HashSHA256(),
                nextServer = newHash.Item2.HashSHA256()
            });
        }

        [HttpPost]
        public IActionResult Play([FromBody] GamePlayModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            if (model.GameId == "mayan")
                return Json(new { redirect = Url.Action("Index", "Mayan", new { wallet = model.Wallet }) });

            return Json(new { error = "invalid_model" });
        }

        [HttpPost]
        public async Task<IActionResult> GetWallet([FromBody] IdModel id)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            var broadcaster = await _broadcastersRepository.GetBroadcasterByAccountId(id.Id);
            if (broadcaster == null)
                return Json(new { error = "invalid_model" });

            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            var balance = await _pointsRepository.GetBalance(account.TwitchId, broadcaster.AccountId);

            return Json(new { balance, name = broadcaster.Points });
        }

        public async Task<IActionResult> GetWallets()
        {
            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            var cursor = await _pointsRepository.GetUserWallets(account.TwitchId);
            var wallets = await cursor.ToListAsync();
            var broadcasters = await _broadcastersRepository.GetBroadcasters(wallets.Select(x => x.BroadcasterAccountId));

            return Json(new { wallets = wallets.Select(x => new { x.Balance, x.BroadcasterAccountId }), broadcasters = broadcasters.Select(x => new { x.AccountId, x.Login, x.Points }) });
        }
    }
}
