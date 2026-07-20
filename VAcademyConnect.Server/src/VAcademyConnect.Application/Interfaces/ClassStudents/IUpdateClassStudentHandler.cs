using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.ClassStudents
{
    public interface IUpdateClassStudentHandler
    {
        Task<AppResponse<ClassStudentResponse>> HandleAsync(ClassStudentRequest request);
    }
}
