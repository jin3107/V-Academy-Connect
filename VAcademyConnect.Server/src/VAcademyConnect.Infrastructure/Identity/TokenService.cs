using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _config;

        public TokenService(UserManager<ApplicationUser> userManager, IRefreshTokenRepository refreshTokenRepository,
            IConfiguration config)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _config = config;
        }

        public async Task<List<Claim>> BuildClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("TenantId", user.TenantId.ToString()!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user, IEnumerable<Claim> claims)
        {
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenModel = new RefreshTokenModel
            {
                UserId = Guid.Parse(user.Id),
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:RefreshTokenExpiresIn"] ?? "10080")),
                IsRevoked = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = user.Email
            };

            await _refreshTokenRepository.AddAsync(refreshTokenModel);

            return (accessToken, refreshToken);
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(int.Parse(_config["Jwt:AccessTokenExpiresIn"] ?? "3600")),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
