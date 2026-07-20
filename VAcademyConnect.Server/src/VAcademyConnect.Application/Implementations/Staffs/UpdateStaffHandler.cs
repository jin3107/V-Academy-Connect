using MayNghien.Infrastructures.Models.Responses;
using System.Security.Claims;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Staffs;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Staffs
{
    public class UpdateStaffHandler : IUpdateStaffHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAccountService _userAccountService;

        public UpdateStaffHandler(IStaffRepository staffRepository, ITenantRepository tenantRepository,
            ICurrentUserService currentUserService, IUserAccountService userAccountService)
        {
            _staffRepository = staffRepository;
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
            _userAccountService = userAccountService;
        }

        public async Task<AppResponse<StaffResponse>> HandleAsync(StaffRequest request)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                if (request?.Id == null || request.Id == new Guid())
                    return result.BuildError("Staff ID is required");

                var staff = await _staffRepository.GetAsync(request.Id);
                if (staff == null || staff.IsDeleted)
                    return result.BuildError("Staff not found");

                if (request.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase) &&
                    !staff.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                {
                    var tenant = staff.TenantId.HasValue
                        ? await _tenantRepository.GetAsync(staff.TenantId.Value)
                        : null;
                    if (tenant == null || tenant.Type != TenantType.Center)
                        return result.BuildError("Only Center tenants can manage Teacher staff. For a Personal tenant, the TenantAdmin itself is the teacher.");
                }

                if ((staff.Email != request.Email || staff.PhoneNumber != request.PhoneNumber) &&
                    await _userAccountService.ExistsByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                    return result.BuildError("Email or phone number already in use");

                var currentEmail = staff.Email;
                staff.Name = request.Name;
                staff.Email = request.Email;
                staff.PhoneNumber = request.PhoneNumber;
                staff.DateOfBirth = request.DateOfBirth;
                staff.Gender = request.Gender;
                staff.Address = request.Address;
                staff.Role = request.Role;
                staff.ModifiedBy = currentUser.Email;
                staff.ModifiedOn = DateTime.UtcNow;

                var identityUser = await _userAccountService.FindByEmailAsync(currentEmail);
                if (identityUser == null)
                    return result.BuildError("Associated user account not found");

                identityUser.Email = request.Email;
                identityUser.PhoneNumber = request.PhoneNumber;

                var (updateSucceeded, updateErrors) = await _userAccountService.UpdateAsync(identityUser);
                if (!updateSucceeded)
                    return result.BuildError("Failed to update user: " + string.Join(", ", updateErrors));

                await _userAccountService.AssignRoleAsync(identityUser, request.Role);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, request.Role),
                };

                await _staffRepository.EditAsync(staff);
                var response = StaffMapper.ToResponse(staff);

                return result.BuildResult(response, "Staff updated successfully");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
