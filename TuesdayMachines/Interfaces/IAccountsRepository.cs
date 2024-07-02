using TuesdayMachines.Dto;
using TuesdayMachines.Models;

namespace TuesdayMachines.Interfaces
{
    public interface IAccountsRepository
    {
        Task<AccountDTO> CreateOrUpdateAccount(TwitchUserResponseModel user, TwitchAuthResponseModel token);
        Task<AccountDTO> GetAccountByTwitchLogin(string login);
        Task<AccountDTO> GetAccountById(string id);
        Task UpdateAccountTokens(string id, TwitchAuthResponseModel token);
    }
}
