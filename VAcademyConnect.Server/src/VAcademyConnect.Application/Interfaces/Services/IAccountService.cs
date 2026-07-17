using MayNghien.Infrastructures.Models.Responses;
using MayNghien.Infrastructures.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;

namespace VAcademyConnect.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AppResponse<List<RoleResponse>>> GetAllRoles();
        Task<AppResponse<LogInResponse>> LoginByGoogleAsync(GoogleLoginRequest request);
        Task<AppResponse<LogInResponse>> LoginAsync(LogInRequest request);
        Task<AppResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<AppResponse<VerifyEmailResponse>> VerifyEmailAsync(VerifyEmailRequest request);
        Task<AppResponse<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<AppResponse<bool>> UpdateProfileAsync(UpdateProfileRequest request);
        Task<AppResponse<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);

        Task LogoutAsync();
    }
}
