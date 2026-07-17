using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Identity;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class RefreshTokenHandler : IRefreshTokenHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(UserManager<ApplicationUser> userManager, IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
        }

        public async Task<AppResponse<RefreshTokenResponse>> HandleAsync(RefreshTokenRequest request)
        {
            var result = new AppResponse<RefreshTokenResponse>();
            try
            {
                var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken!);
                if (storedToken == null)
                    return result.BuildError("Invalid refresh token");

                if (storedToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    storedToken.IsRevoked = true;
                    _refreshTokenRepository.Edit(storedToken);
                    return result.BuildError("Refresh token has expired");
                }

                var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString()!);
                if (user == null)
                    return result.BuildError("User not found");

                var claims = await _tokenService.BuildClaimsAsync(user);
                var (newAccessToken, newRefreshToken) = await _tokenService.GenerateTokensAsync(user, claims);

                storedToken.IsRevoked = true;
                _refreshTokenRepository.Edit(storedToken);

                var response = new RefreshTokenResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Expiration = DateTime.UtcNow.AddHours(1)
                };

                return result.BuildResult(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
