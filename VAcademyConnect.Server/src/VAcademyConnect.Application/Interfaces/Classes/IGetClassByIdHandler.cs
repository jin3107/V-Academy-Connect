using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Classes
{
    public interface IGetClassByIdHandler
    {
        Task<AppResponse<ClassResponse>> HandleAsync(Guid id);
    }
}
