
using Microsoft.AspNetCore.Mvc;
using TuesdayMachines.Interfaces;

namespace TuesdayMachines.Filters
{
    public class HomeEndpointFilter : IEndpointFilter
    {
        private IUserAuthentication _userAuthentication;

        public HomeEndpointFilter(IUserAuthentication userAuthentication)
        {
            _userAuthentication = userAuthentication;
        }


        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var account = await _userAuthentication.GetAuthenticatedUser(context.HttpContext);
            if (account == null)
            {
                return Results.Json(new { error = "not_authenticated" });
            }

            context.HttpContext.Items["userAccount"] = account;

            return await next(context);
        }
    }
}
