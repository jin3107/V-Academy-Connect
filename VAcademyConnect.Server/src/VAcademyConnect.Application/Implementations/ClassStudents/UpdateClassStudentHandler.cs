using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.ClassStudents
{
    public class UpdateClassStudentHandler : IUpdateClassStudentHandler
    {
        public Task<AppResponse<ClassStudentResponse>> HandleAsync(ClassStudentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
