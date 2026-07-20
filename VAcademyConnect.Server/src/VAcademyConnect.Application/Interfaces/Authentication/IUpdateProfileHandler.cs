using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;

namespace VAcademyConnect.Application.Interfaces.Authentication
{
    public interface IUpdateProfileHandler
    {
        Task<AppResponse<UpdateProfileResponse>> HandleAsync(UpdateProfileRequest request);
    }
}
