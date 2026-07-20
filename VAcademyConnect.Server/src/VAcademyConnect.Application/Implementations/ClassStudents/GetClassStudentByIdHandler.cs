using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.ClassStudents
{
    public class GetClassStudentByIdHandler : IGetClassStudentByIdHandler
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetClassStudentByIdHandler(IClassStudentRepository classStudentRepository, ICurrentUserService currentUserService)
        {
            _classStudentRepository = classStudentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<ClassStudentResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<ClassStudentResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var classStudent = await _classStudentRepository.FindByAsync(c => c.Id == id)
                    .Include(c => c.Student).Include(c => c.Class).FirstOrDefaultAsync();
                if (classStudent == null || classStudent.IsDeleted == true)
                    return result.BuildError("Student in class was not found or has been deleted.");

                var response = ClassStudentMapper.ToResponse(classStudent);
                return result.BuildResult(response, "Student in class has been found");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
