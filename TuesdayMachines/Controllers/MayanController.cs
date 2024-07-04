using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TuesdayMachines.ActionFilters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;
using TuesdayMachines.Services;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(HomeActionFilter))]
    public class MayanController : Controller
    {
        private readonly long[] _betsAllowedValues = [25, 50, 75, 100, 125, 250, 500, 750, 1000, 1250, 2500, 5000, 10000];

        private readonly IMayanGame _mayanGame;
        private readonly IPointsRepository _pointsRepository;
        private readonly IUserFairPlay _userFairPlay;
        private readonly ISpinsRepository _spinsRepository;

        public MayanController(IMayanGame mayanGame, IPointsRepository pointsRepository, IUserFairPlay userFairPlay, ISpinsRepository spinsRepository)
        {
            _mayanGame = mayanGame;
            _pointsRepository = pointsRepository;
            _userFairPlay = userFairPlay;
            _spinsRepository = spinsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("spins")]
        public async Task<IActionResult> Play([FromBody] DefaultGamePlayModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

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
                return Json(new { error = "invalid_model" });

            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            var result = _pointsRepository.TakePoints(account.TwitchId, model.Wallet, model.Bet);
            if (!result.Success)
                return Json(new { error = "not_sufficient_funds" });

            var roundInfo = await _userFairPlay.GetUserSeedRoundInfo(account.Id);
            var gameResult = _mayanGame.SimulateGame(roundInfo.Client, roundInfo.Server, roundInfo.Nonce, model.Bet);

            if (gameResult.TotalWin > 0)
            {
                _pointsRepository.AddPoints(account.TwitchId, model.Wallet, gameResult.TotalWin);

                var X = (double)gameResult.TotalWin / gameResult.Bet;
                if (X >= 10.0)
                {
                    await _spinsRepository.AddSpinStatLog(new Dto.SpinStatDTO()
                    {
                        AccountId = account.Id,
                        Bet = model.Bet,
                        Game = "mayan",
                        Wallet = model.Wallet,
                        Win = gameResult.TotalWin,
                        WinX = (long)X
                    });
                }
            }

            /*
            await _spinsRepository.AddSpinLog(new Dto.SpinDTO()
            {
                AccountId = account.Id,
                Bet = model.Bet,
                Result = gameResult.TotalWin,
                Game = $"mayan_{_mayanGame.GetVersion()}",
                Seed = $"{roundInfo.Client}:{roundInfo.Server}:{roundInfo.Nonce}",
                Wallet = model.Wallet
            });
            */

            gameResult.CurrentBalance = (await _pointsRepository.GetBalance(account.TwitchId, model.Wallet)).Balance;

            return Json(gameResult);
        }
    }
}