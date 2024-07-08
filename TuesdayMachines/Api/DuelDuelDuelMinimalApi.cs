using Microsoft.AspNetCore.Mvc;
using TuesdayMachines.Filters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;

namespace TuesdayMachines.Api
{
	public static class DuelDuelDuelMinimalApi
	{
		public static void MapDuelDuelDuelEndpoints(this IEndpointRouteBuilder app)
		{
			var duel = app.MapGroup("duelduelduel").AddEndpointFilter<HomeEndpointFilter>();

			duel.MapPost("play", Play)
				.RequireRateLimiting("spins")
				.AddEndpointFilter<ValidationFilter<DefaultGamePlayModel>>();
		}

		public static Task<IResult> Play([FromBody] DefaultGamePlayModel model, IUserFairPlay userFairPlay, IDuelDuelDuelGame duelDuelDuelGame, HttpContext context)
		{
			var account = context.Items["userAccount"] as Dto.AccountDTO;
			var roundInfo = userFairPlay.GetUserSeedRoundInfo(account.Id);
			var gameResult = duelDuelDuelGame.SimulateGame(roundInfo.Client, roundInfo.Server, roundInfo.Nonce, model.Bet);

			return Task.FromResult(Results.Json(gameResult));
		}
	}
}
