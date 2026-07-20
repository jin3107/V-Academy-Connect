using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Classes
{
    public interface IUpdateClassHandler
    {
        Task<AppResponse<ClassResponse>> HandleAsync(ClassRequest request);
    }
}
