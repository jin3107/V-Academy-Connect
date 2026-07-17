using System.Security.Claims;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Interfaces
{
    public interface ITokenService
    {
        Task<List<Claim>> BuildClaimsAsync(ApplicationUser user);
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user, IEnumerable<Claim> claims);
    }
}
