using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Concurrent;
using System.ComponentModel;
using TuesdayMachines.Filters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(HomeActionFilter))]
    public class MinesController : Controller
    {
        private static readonly long[] _betsAllowedValues = [5, 10, 25, 50, 100, 150, 250, 500, 1000, 1500, 2000, 5000, 10000, 15000, 20000];
        private static readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();

        private readonly IUserFairPlay _userFairPlay;
        private readonly IPointsRepository _pointsRepository;
        private readonly IMinesGame _minesGame;
        private readonly ISpinsRepository _spinsRepository;

        public MinesController(IUserFairPlay userFairPlay, IPointsRepository pointsRepository, IMinesGame minesGame, ISpinsRepository spinsRepository)
        {
            _userFairPlay = userFairPlay;
            _pointsRepository = pointsRepository;
            _minesGame = minesGame;
            _spinsRepository = spinsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("spins")]
        public IActionResult Play([FromBody] MinesGamePlayModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            var _lock = _locks.GetOrAdd(account.Id, new object());

            lock (_lock)
            {
                var activeGame = _userFairPlay.GetMinesActiveGame(account.Id);
                if (model is MinesGamePlayStartGameModel gameplay)
                {
                    if (activeGame != null)
                        return Json(new { error = "game_exsits" });

                    if (gameplay.Mines < 1 || gameplay.Mines > 24)
                        return Json(new { error = "invalid_model" });

                    bool found = false;
                    for (var i = 0; i < _betsAllowedValues.Length; i++)
                    {
                        if (gameplay.Bet == _betsAllowedValues[i])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        return Json(new { error = "invalid_model" });

                    var result = _pointsRepository.TakePoints(account.TwitchId, gameplay.Wallet, gameplay.Bet);
                    if (!result.Success)
                        return Json(new { error = "not_sufficient_funds" });

                    var roundInfo = _userFairPlay.GetUserSeedRoundInfo(account.Id);
                    var resultGame = _minesGame.SimulateGame(roundInfo.Client, roundInfo.Server, roundInfo.Nonce, gameplay.Mines);

                    _userFairPlay.AddMinesGame(account.Id, gameplay.Wallet, gameplay.Bet, resultGame.Bombs);

                    return Json(new { success = "game_started", balance = result.Balance });
                }
                else if (model is MinesGamePlayRevealTileModel revealTileModel)
                {
                    if (activeGame == null)
                        return Json(new { error = "game_dosent_exsits" });

                    if (revealTileModel.Index < 0 || revealTileModel.Index > 24)
                        return Json(new { error = "invalid_model" });

                    bool alreadyPicked = false;
                    for (var i = 0; i < activeGame.Picked.Length; i++)
                    {
                        if (activeGame.Picked[i] == revealTileModel.Index)
                        {
                            alreadyPicked = true;
                            break;
                        }
                    }

                    if (alreadyPicked)
                        return Json(new { isMine = false });

                    bool pickedBomb = false;
                    for (var i = 0; i < activeGame.Bombs.Length; i++)
                    {
                        if (activeGame.Bombs[i] == revealTileModel.Index)
                        {
                            pickedBomb = true;
                            break;
                        }
                    }

                    if (pickedBomb)
                    {
                        _userFairPlay.RemoveMinesGame(account.Id);
                        _locks.TryRemove(account.Id, out _);

                        return Json(new { isMine = true, mines = activeGame.Bombs, picked = activeGame.Picked });
                    }

                    //Winner no more tiles
                    //
                    if (activeGame.Picked.Length + 1 == 25 - activeGame.Bombs.Length)
                    {
                        var multi = _minesGame.GenerateMinesMulitpler(activeGame.Picked.Length + 1, activeGame.Bombs.Length);
                        var win = (long)(activeGame.Bet * multi);

                        _pointsRepository.AddPoints(account.TwitchId, activeGame.WalletId, win);
                        _userFairPlay.RemoveMinesGame(account.Id);
                        _locks.TryRemove(account.Id, out _);

                        if (multi >= 10.0)
                        {
                            _spinsRepository.AddSpinStatLog(new Dto.SpinStatDTO()
                            {
                                AccountId = account.Id,
                                Bet = activeGame.Bet,
                                Game = "mines",
                                Wallet = activeGame.WalletId,
                                Win = win,
                                WinX = (long)multi
                            });
                        }

                        return Json(new { isMine = false, winnings = win, multiplier = multi });
                    }

                    _userFairPlay.UpdateMinesGame(account.Id, revealTileModel.Index);

                    return Json(new { isMine = false });
                }
                else if (model is MinesGamePlayCashoutModel cashoutModel)
                {
                    if (activeGame == null)
                        return Json(new { error = "game_dosent_exsits" });

                    if (activeGame.Picked.Length == 0)
                        return Json(new { error = "no_tiles_selected" });

                    var multi = _minesGame.GenerateMinesMulitpler(activeGame.Picked.Length, activeGame.Bombs.Length);
                    var win = (long)(activeGame.Bet * multi);

                    _pointsRepository.AddPoints(account.TwitchId, activeGame.WalletId, win);
                    _userFairPlay.RemoveMinesGame(account.Id);
                    _locks.TryRemove(account.Id, out _);

                    if (multi >= 10.0)
                    {
                        _spinsRepository.AddSpinStatLog(new Dto.SpinStatDTO()
                        {
                            AccountId = account.Id,
                            Bet = activeGame.Bet,
                            Game = "mines",
                            Wallet = activeGame.WalletId,
                            Win = win,
                            WinX = (long)multi
                        });
                    }

                    return Json(new { winnings = win, multiplier = multi, mines = activeGame.Bombs, picked = activeGame.Picked });
                }
                else
                {
                    if (activeGame == null)
                        return Json(new { status = "idle" });

                    return Json(new
                    {
                        game = new
                        {
                            bet = activeGame.Bet,
                            picked = activeGame.Picked,
                            mines = activeGame.Bombs.Length
                        }
                    });
                }
            }
        }
    }
}
