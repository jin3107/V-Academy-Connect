using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Students;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.Students
{
    public class CreateStudentHandler : ICreateStudentHandler
    {
        private readonly IUserAccountService _userAccountService;
        private readonly ICurrentUserService _currentUserService;

        public CreateStudentHandler(IUserAccountService userAccountService, ICurrentUserService currentUserService)
        {
            _userAccountService = userAccountService;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<StudentResponse>> HandleAsync(StudentRequest request)
        {
            var result = new AppResponse<StudentResponse>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorize.");

                var roles = await _currentUserService.GetRolesAsync(currentUser);
                var tenantId = roles.Contains("SuperAdmin") ? request.TenantId : currentUser.TenantId;
                if (!tenantId.HasValue)
                    return result.BuildError("TenantId is required.");

                if (await _userAccountService.ExistsByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                    return result.BuildError("A user with this email or phone number already exists.");

                var newStudent = new ApplicationUser
                {
                    FullName = request.Name,
                    UserName = request.Email,
                    Email = request.Email,
                    TenantId = tenantId,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = request.PhoneNumber,
                };

                var (createSucceeded, createErrors, createdUser) = await _userAccountService.CreateAsync(newStudent, request.Password);
                if (!createSucceeded || createdUser == null)
                    return result.BuildError("Cannot create student: " + string.Join(", ", createErrors));

                await _userAccountService.AssignRoleAsync(createdUser, "Student");

                var response = StudentMapper.ToResponse(createdUser);
                result.BuildResult(response, "Student has been created successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
