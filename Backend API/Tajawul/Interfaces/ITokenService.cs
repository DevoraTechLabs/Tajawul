using System.Security.Claims;
using Tajawul.Models.Domain.Users;

namespace Tajawul.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Person appUser, IList<string> roles);
        string CreateRefreshToken(Person appUser, IList<string> roles);
        ClaimsPrincipal? ValidateRefreshToken(string refreshToken);
        public DateTime GetExpirationDate(string token);

    }

}

