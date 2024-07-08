using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Concurrent;
using TuesdayMachines.Filters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;

namespace TuesdayMachines.Api
{
    public static class MinesMinimalApi
    {
        private static readonly long[] _betsAllowedValues = [5, 10, 25, 50, 100, 150, 250, 500, 1000, 1500, 2000, 5000, 10000, 15000, 20000];
        private static readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();
        public static void MapMinesEndpoints(this IEndpointRouteBuilder app)
        {
            var mines = app.MapGroup("mines").AddEndpointFilter<HomeEndpointFilter>();

            mines.MapPost("play", Play)
                .RequireRateLimiting("spins")
                .AddEndpointFilter<ValidationFilter<MinesGamePlayModel>>();
        }

        public static IResult Play([FromBody] MinesGamePlayModel model, IOnlinePlayersCounter onlinePlayersCounter, IPointsRepository pointsRepository, IUserFairPlay userFairPlay, IMinesGame minesGame, ISpinsRepository spinsRepository, HttpContext context)
        {
            var account = context.Items["userAccount"] as Dto.AccountDTO;
            var _lock = _locks.GetOrAdd(account.Id, new object());

            lock (_lock)
            {
                var activeGame = userFairPlay.GetMinesActiveGame(account.Id);
                if (model is MinesGamePlayStartGameModel gameplay)
                {
                    if (activeGame != null)
                        return Results.Json(new { error = "game_exsits" });

                    if (gameplay.Mines < 1 || gameplay.Mines > 24)
                        return Results.Json(new { error = "invalid_model" });

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
                        return Results.Json(new { error = "invalid_model" });

                    var result = pointsRepository.TakePoints(account.TwitchId, gameplay.Wallet, gameplay.Bet);
                    if (!result.Success)
                        return Results.Json(new { error = "not_sufficient_funds" });

                    onlinePlayersCounter.AddPlayer("mines", account.Id, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                    var roundInfo = userFairPlay.GetUserSeedRoundInfo(account.Id);
                    var bombs = minesGame.SimulateGame(roundInfo.Client, roundInfo.Server, roundInfo.Nonce, gameplay.Mines);

                    userFairPlay.AddMinesGame(account.Id, gameplay.Wallet, gameplay.Bet, bombs);

                    return Results.Json(new { success = "game_started", balance = result.Balance });
                }
                else if (model is MinesGamePlayRevealTileModel revealTileModel)
                {
                    if (activeGame == null)
                        return Results.Json(new { error = "game_dosent_exsits" });

                    if (revealTileModel.Index < 0 || revealTileModel.Index > 24)
                        return Results.Json(new { error = "invalid_model" });

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
                        return Results.Json(new { isMine = false });

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
                        userFairPlay.RemoveMinesGame(account.Id);
                        _locks.TryRemove(account.Id, out _);

                        return Results.Json(new { isMine = true, mines = activeGame.Bombs, picked = activeGame.Picked });
                    }

                    //Winner no more tiles
                    //
                    if (activeGame.Picked.Length + 1 == 25 - activeGame.Bombs.Length)
                    {
                        var multi = minesGame.GenerateMinesMulitpler(activeGame.Picked.Length + 1, activeGame.Bombs.Length);
                        var win = (long)(activeGame.Bet * multi);

                        var result = pointsRepository.AddPoints(account.TwitchId, activeGame.WalletId, win);
                        userFairPlay.RemoveMinesGame(account.Id);
                        _locks.TryRemove(account.Id, out _);

                        if (multi >= 10.0)
                        {
                            spinsRepository.AddSpinStatLog(new Dto.SpinStatDTO()
                            {
                                AccountId = account.Id,
                                Bet = activeGame.Bet,
                                Game = "mines",
                                Wallet = activeGame.WalletId,
                                Win = win,
                                WinX = (long)multi
                            });
                        }

                        return Results.Json(new { isMine = false, winnings = win, multiplier = multi, balance = result.Balance });
                    }

                    userFairPlay.UpdateMinesGame(account.Id, revealTileModel.Index);

                    return Results.Json(new { isMine = false });
                }
                else if (model is MinesGamePlayCashoutModel cashoutModel)
                {
                    if (activeGame == null)
                        return Results.Json(new { error = "game_dosent_exsits" });

                    if (activeGame.Picked.Length == 0)
                        return Results.Json(new { error = "no_tiles_selected" });

                    var multi = minesGame.GenerateMinesMulitpler(activeGame.Picked.Length, activeGame.Bombs.Length);
                    var win = (long)(activeGame.Bet * multi);

                    var result = pointsRepository.AddPoints(account.TwitchId, activeGame.WalletId, win);
                    userFairPlay.RemoveMinesGame(account.Id);
                    _locks.TryRemove(account.Id, out _);

                    if (multi >= 10.0)
                    {
                        spinsRepository.AddSpinStatLog(new Dto.SpinStatDTO()
                        {
                            AccountId = account.Id,
                            Bet = activeGame.Bet,
                            Game = "mines",
                            Wallet = activeGame.WalletId,
                            Win = win,
                            WinX = (long)multi
                        });
                    }

                    return Results.Json(new { winnings = win, multiplier = multi, mines = activeGame.Bombs, picked = activeGame.Picked, balance = result.Balance });
                }
                else
                {
                    if (activeGame == null)
                        return Results.Json(new { status = "idle" });

                    return Results.Json(new
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
