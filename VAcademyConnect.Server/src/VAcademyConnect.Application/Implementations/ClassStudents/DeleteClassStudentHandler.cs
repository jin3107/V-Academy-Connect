using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.Application.Interfaces.Repositories;

namespace VAcademyConnect.Application.Implementations.ClassStudents
{
    public class DeleteClassStudentHandler : IDeleteClassStudentHandler
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteClassStudentHandler(IClassStudentRepository classStudentRepository, ICurrentUserService currentUserService)
        {
            _classStudentRepository = classStudentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<string>> HandleAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var classStudent = await _classStudentRepository.GetAsync(id);
                if (classStudent == null || classStudent.IsDeleted == true)
                    return result.BuildError("Student in class was not found or has been deleted.");

                classStudent.IsDeleted = true;
                await _classStudentRepository.EditAsync(classStudent);
                result.BuildResult("Student in class has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
