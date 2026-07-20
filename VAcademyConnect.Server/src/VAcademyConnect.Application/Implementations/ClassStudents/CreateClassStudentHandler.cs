using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.ClassStudents
{
    public class CreateClassStudentHandler : ICreateClassStudentHandler
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateClassStudentHandler(IClassStudentRepository classStudentRepository, ICurrentUserService currentUserService)
        {
            _classStudentRepository = classStudentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<ClassStudentResponse>> HandleAsync(ClassStudentRequest request)
        {
            var result = new AppResponse<ClassStudentResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var existingEnrollment = await _classStudentRepository
                    .FindByAsync(cs => cs.ClassId == request.ClassId && cs.StudentId == request.StudentId)
                    .FirstOrDefaultAsync();

                if (existingEnrollment != null && !existingEnrollment.IsDeleted)
                    return result.BuildError("Student is already enrolled in this class.");

                var newStudentInClass = ClassStudentMapper.ToEntity(request);
                newStudentInClass.Id = Guid.NewGuid();
                newStudentInClass.CreatedBy = user.Email;
                newStudentInClass.CreatedOn = DateTime.UtcNow;

                await _classStudentRepository.AddAsync(newStudentInClass);

                var createdEntity = await _classStudentRepository.FindByAsync(cs => cs.Id == newStudentInClass.Id)
                    .Include(cs => cs.Student).FirstOrDefaultAsync();

                var response = ClassStudentMapper.ToResponse(createdEntity!);
                result.BuildResult(response, "Student has been added to class successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
