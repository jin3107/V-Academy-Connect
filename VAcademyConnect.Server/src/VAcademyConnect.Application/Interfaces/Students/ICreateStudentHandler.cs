using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Students
{
    public interface ICreateStudentHandler
    {
        Task<AppResponse<StudentResponse>> HandleAsync(StudentRequest request);
    }
}
