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

        public HomeController(IUserAuthentication userAuthentication, IGamesRepository gamesRepository)
        {
            _userAuthentication = userAuthentication;
            _gamesRepository = gamesRepository;
        }

        public async Task<IActionResult> Index()
        {
            HomeIndexModel model = new HomeIndexModel();
            model.Games = await (await _gamesRepository.GetGames()).ToListAsync();
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
