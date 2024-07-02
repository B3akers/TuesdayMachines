using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TuesdayMachines.ActionFilters;
using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;
using TuesdayMachines.Services;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(BroadcasterActionFilter))]
    public class BroadcasterController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly IBroadcastersRepository _broadcastersRepository;
        private readonly IPointsRepository _pointsRepository;
        private readonly IGamesRepository _gamesRepository;

        public BroadcasterController(IBroadcastersRepository broadcastersRepository, DatabaseService databaseService, IPointsRepository pointsRepository, IGamesRepository gamesRepository)
        {
            _broadcastersRepository = broadcastersRepository;
            _databaseService = databaseService;
            _pointsRepository = pointsRepository;
            _gamesRepository = gamesRepository;
        }

        [HttpGet("Broadcaster/Index")]
        [HttpGet("Broadcaster/Index/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            return View();
        }

        [HttpGet("Broadcaster/Accounts")]
        [HttpGet("Broadcaster/Accounts/{id}")]
        public async Task<IActionResult> Accounts(string id)
        {
            var broadcaster = await _broadcastersRepository.GetBroadcasterByAccountId(GetRealId(id));
            if (broadcaster == null)
                return Redirect(Url.Action("Index", "Home"));
            return View(broadcaster);
        }


        [HttpGet("Broadcaster/Settings")]
        [HttpGet("Broadcaster/Settings/{id}")]
        public async Task<IActionResult> Settings(string id)
        {
            var broadcaster = await _broadcastersRepository.GetBroadcasterByAccountId(GetRealId(id));
            if (broadcaster == null)
                return Redirect(Url.Action("Index", "Home"));
            return View(broadcaster);
        }

        [HttpPost("Broadcaster/GetAccountWallet")]
        [HttpPost("Broadcaster/GetAccountWallet/{id}")]
        public async Task<IActionResult> GetAccountWallet(string id, [FromBody] TwitchIdModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            var broadcaster = await _broadcastersRepository.GetBroadcasterByAccountId(GetRealId(id));
            if (broadcaster == null)
                return Json(new { error = "invalid_model" });

            var balance = await _pointsRepository.GetBalance(model.TwitchId, broadcaster.AccountId);

            return Json(new
            {
                model.TwitchId,
                balance
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetAccounts([FromBody] DataTableProcessParametrs parametrs)
        {
            var result = await Datatables.StartPaging<AccountDTO>(parametrs).AddGlobalFilterField(x => x.TwitchId, DatatableFileInfoSerach.Eq).AddGlobalFilterField(x => x.TwitchLogin, DatatableFileInfoSerach.Eq).ApplyGlobalFilter().Execute(_databaseService.GetAccounts());
            return Json(new { parametrs.Draw, RecordsFiltered = result.Item2, RecordsTotal = result.Item3, data = result.Item1.Select(x => new { x.Id, x.TwitchId, x.TwitchLogin, x.CreationTime }) });
        }

        [HttpPost]
        public async Task<IActionResult> EditAccountWallet([FromBody] EditAccountWalletModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            await _pointsRepository.SetPoints(model.TwitchId, GetRealId(model.Id), model.Points);

            return Json(new { success = "updated" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePointsSettings([FromBody] ChangeBroadcasterSettingsModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "invalid_model" });

            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            if (!account.IsAdmin() && model.AccountId != account.Id)
                return Json(new { error = "invalid_model" });

            await _broadcastersRepository.UpdateBroadcaster(model);

            return Json(new { success = "updated" });
        }

        private string GetRealId(string id)
        {
            var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
            if (string.IsNullOrEmpty(id) ||
                !account.IsAdmin())
            {
                id = account.Id;
            }

            return id;
        }
    }
}
