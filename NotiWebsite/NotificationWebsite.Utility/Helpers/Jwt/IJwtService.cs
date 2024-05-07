using System.Security.Claims;
using NotificationWebsite.Models;

namespace NotificationWebsite.Utility.Helpers.Jwt
{
    public interface IJwtService
    {
        public string Generate(int id);

        public Task<User> GetUserByToken(string token);

        public ClaimsPrincipal GetClaimsPrincipalFromToken(string token);
    }
}