using TuesdayMachines.Dto;

namespace TuesdayMachines.Interfaces
{
    public interface IUserAuthentication
    {
        public Task<AccountDTO> GetAuthenticatedUser(HttpContext context);
        public Task AuthorizeForUser(HttpContext context, string accountId, bool permanent);
        public Task LogoutUser(HttpContext context);
        public Task LogoutUser(string accountId);
    }
}
