using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Authentication.Requests;

namespace VAcademyConnect.Application.Interfaces.Authentication
{
    public interface IVerifyOtpHandler
    {
        Task<AppResponse<bool>> HandleAsync(VerifyOtpRequest request);
    }
}
