using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.ClassStudents
{
    public interface IDeleteClassStudentHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
