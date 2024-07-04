using Microsoft.AspNetCore.Mvc;
using TuesdayMachines.ActionFilters;
using TuesdayMachines.WebSockets;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(HomeActionFilter))]
    public class RouletteController : Controller
    {
        private readonly WebSocketRouletteHandler _handler;
        public RouletteController(WebSocketRouletteHandler handler)
        {
            _handler = handler;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task Ws()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var account = HttpContext.Items["userAccount"] as Dto.AccountDTO;
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                await _handler.Connection(account, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
