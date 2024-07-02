using TuesdayMachines.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using TuesdayMachines.ActionFilters;

namespace TuesdayMachines.Controllers
{
    [TypeFilter(typeof(LoginActionFilter))]
    public class LoginController : Controller
    {
        private readonly ITwitchApi _twitchApi;
        private readonly IAccountsRepository _accountRepository;
        private readonly IUserAuthentication _userAuthentication;
        public LoginController(ITwitchApi twitchApi, IAccountsRepository accountRepository, IUserAuthentication userAuthentication)
        {
            _twitchApi = twitchApi;
            _accountRepository = accountRepository;
            _userAuthentication = userAuthentication;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TwitchLogin(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return new RedirectResult(Url.Action("Index", "Login", new { error = "twitch_connect_error" }), false);

                var twitchToken = await _twitchApi.TwitchAuthorization(code, Url.Action("TwitchLogin", "Login", null, Request.Scheme));
                int validScopes = 0;
                foreach (var token in twitchToken.Scope)
                {
                    if (token == "user:read:email"
                        || token == "moderator:read:chatters"
                        || token == "channel:read:subscriptions")
                        validScopes++;
                }

                if (validScopes < 3)
                    return new RedirectResult(Url.Action("Index", "Login", new { error = "twitch_invalid_scope" }), false);

                var userInfo = await _twitchApi.TwitchGetUserInfo(twitchToken.AccessToken);
                if (string.IsNullOrEmpty(userInfo.Email))
                    return new RedirectResult(Url.Action("Index", "Login", new { error = "twitch_invalid_email" }), false);

                var account = await _accountRepository.CreateOrUpdateAccount(userInfo, twitchToken);

                await _userAuthentication.AuthorizeForUser(HttpContext, account.Id, true);
                return new RedirectResult(Url.Action("Index", "Home", new { success = "twitch_connect_success" }), false);
            }
            catch { }

            return new RedirectResult(Url.Action("Index", "Login", new { error = "twitch_connect_error" }), false);
        }
    }
}
