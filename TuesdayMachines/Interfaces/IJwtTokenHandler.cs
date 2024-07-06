using System.Security.Claims;

namespace TuesdayMachines.Interfaces
{
    public interface IJwtTokenHandler
    {
        public string GenerateToken(ClaimsIdentity claims, DateTime? expires);
        public bool ValidateToken(string token);
    }
}
