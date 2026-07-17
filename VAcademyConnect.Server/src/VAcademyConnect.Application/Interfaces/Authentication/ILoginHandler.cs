using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;

namespace VAcademyConnect.Application.Interfaces.Authentication
{
    public interface ILoginHandler
    {
        Task<AppResponse<LogInResponse>> HandleAsync(LogInRequest request);
    }
}
