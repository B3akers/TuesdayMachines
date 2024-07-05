using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Diagnostics;
using TuesdayMachines.ActionFilters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(HomeActionFilter))]
    public class HomeController : Controller
    {
        private readonly IUserAuthentication _userAuthentication;
        private readonly IGamesRepository _gamesRepository;
        private readonly IBroadcastersRepository _broadcastersRepository;

        public HomeController(IUserAuthentication userAuthentication, IGamesRepository gamesRepository, IBroadcastersRepository broadcastersRepository)
        {
            _userAuthentication = userAuthentication;
            _gamesRepository = gamesRepository;
            _broadcastersRepository = broadcastersRepository;
        }

        public async Task<IActionResult> Index()
        {
            HomeIndexModel model = new HomeIndexModel();
            model.Games = await _gamesRepository.GetGames();
            return View(model);
        }

        public async Task<IActionResult> Ranking()
        {
            HomeRankingModel model = new HomeRankingModel();
            model.Broadcasters = await _broadcastersRepository.GetBroadcasters();
            return View(model);
        }

        public async Task<IActionResult> Statistics()
        {
            HomeIndexModel model = new HomeIndexModel();
            model.Games = await _gamesRepository.GetGames();
            model.Broadcasters = await _broadcastersRepository.GetBroadcasters();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userAuthentication.LogoutUser(HttpContext);

            return new RedirectResult(Url.Action("Index", "Login"), false);
        }
    }
}
