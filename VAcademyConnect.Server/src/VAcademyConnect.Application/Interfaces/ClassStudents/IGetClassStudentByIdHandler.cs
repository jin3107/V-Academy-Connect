using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.ClassStudents
{
    public interface IGetClassStudentByIdHandler
    {
        Task<AppResponse<ClassStudentResponse>> HandleAsync(Guid id);
    }
}
