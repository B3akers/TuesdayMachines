﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TuesdayMachines.Interfaces;

namespace TuesdayMachines.ActionFilters
{
    public class BroadcasterActionFilter : IAsyncActionFilter
    {
        private IUserAuthentication _userAuthentication;

        public BroadcasterActionFilter(IUserAuthentication userAuthentication)
        {
            _userAuthentication = userAuthentication;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var account = await _userAuthentication.GetAuthenticatedUser(context.HttpContext);
            if (account == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" })) { Permanent = false };
                return;
            }

            if ((account.AccountType & (1 << 1)) == 0)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" })) { Permanent = false };
                return;
            }

            context.HttpContext.Items["userAccount"] = account;

            await next();
        }
    }
}
