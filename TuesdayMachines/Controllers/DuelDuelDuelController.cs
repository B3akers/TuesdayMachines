using Microsoft.AspNetCore.Mvc;
using TuesdayMachines.Filters;

namespace TuesdayMachines.Controllers
{
	[TypeFilter(typeof(HomeActionFilter))]
	public class DuelDuelDuelController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
