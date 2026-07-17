using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;

namespace VAcademyConnect.Application.Interfaces.Authentication
{
    public interface IRefreshTokenHandler
    {
        Task<AppResponse<RefreshTokenResponse>> HandleAsync(RefreshTokenRequest request);
    }
}
