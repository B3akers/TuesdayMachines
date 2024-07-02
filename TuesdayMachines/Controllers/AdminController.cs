using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TuesdayMachines.ActionFilters;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;
using TuesdayMachines.Services;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(AdminActionFilter))]
    public class AdminController : Controller
    {
        private readonly IBroadcastersRepository _broadcastersRepositoryService;
        private readonly IAccountsRepository _accountRepository;
        private readonly IGamesRepository _gamesRepository;

        public AdminController(IBroadcastersRepository broadcastersRepositoryService, IAccountsRepository accountRepository, IGamesRepository gamesRepository)
        {
            _broadcastersRepositoryService = broadcastersRepositoryService;
            _accountRepository = accountRepository;
            _gamesRepository = gamesRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Broadcasters()
        {
            return View();
        }

        public IActionResult Games()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBroadcasters()
        {
            var accounts = await _broadcastersRepositoryService.GetBroadcasters();

            return Json(new { data = await accounts.ToListAsync() });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGame([FromBody] AddGameModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            await _gamesRepository.UpdateOrCreateGame(model);

            return Json(new { success = "updated" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBroadcaster([FromBody] IdModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            await _broadcastersRepositoryService.DeleteBroadcaster(model.Id);

            return Json(new { success = "deleted" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGame([FromBody] IdModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            await _gamesRepository.DeleteGame(model.Id);

            return Json(new { success = "deleted" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBroadcaster([FromBody] AddBroadcasterModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            var account = await _accountRepository.GetAccountByTwitchLogin(model.Login);
            if (account == null)
                return Json(new { error = "account_not_found" });

            if (await _broadcastersRepositoryService.GetBroadcasterByAccountId(account.Id) != null)
                return Json(new { error = "already_exists" });

            var result = await _broadcastersRepositoryService.CreateBroadcaster(account, model.Points);

            return Json(new { success = "created", id = result.Id });
        }
    }
}
