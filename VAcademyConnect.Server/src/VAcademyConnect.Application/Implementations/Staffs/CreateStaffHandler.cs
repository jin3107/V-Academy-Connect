using MayNghien.Infrastructures.Models.Responses;
using System.Security.Claims;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Staffs;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.Staffs
{
    public class CreateStaffHandler : ICreateStaffHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAccountService _userAccountService;

        public CreateStaffHandler(IStaffRepository staffRepository, ITenantRepository tenantRepository,
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

                if (request.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                {
                    var tenant = currentUser.TenantId.HasValue
                        ? await _tenantRepository.GetAsync(currentUser.TenantId.Value)
                        : null;
                    if (tenant == null || tenant.Type != TenantType.Center)
                        return result.BuildError("Only Center tenants can manage Teacher staff. For a Personal tenant, the TenantAdmin itself is the teacher.");
                }

                var createStaff = await CreateStaff(request, currentUser);
                if (createStaff == null)
                    return result.BuildError("Failed to create staff.");

                var (createSucceeded, createErrors, createdUser) = await CreateUser(request, currentUser);
                if (!createSucceeded)
                {
                    await _staffRepository.DeleteAsync(createStaff);

                    if (createdUser != null)
                        await _userAccountService.DeleteAsync(createdUser);

                    return result.BuildError("Cannot create user: " + string.Join(", ", createErrors));
                }

                var identityUser = await _userAccountService.FindByEmailAsync(request.Email);
                if (identityUser == null)
                    return result.BuildError("Failed to retrieve created Staff.");

                await _userAccountService.AssignRoleAsync(identityUser, request.Role);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, request.Role),
                };

                var response = StaffMapper.ToResponse(createStaff);
                return result.BuildResult(response, "Staff created successfully.");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        private async Task<(bool Succeeded, IEnumerable<string> Errors, ApplicationUser? User)> CreateUser(StaffRequest request, ApplicationUser currentUser)
        {
            var identityUser = new ApplicationUser
            {
                FullName = request.Name,
                UserName = request.Email,
                Email = request.Email,
                TenantId = currentUser.TenantId,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = request.PhoneNumber,
                Role = Role.Staff
            };

            return await _userAccountService.CreateAsync(identityUser, request.Password);
        }

        private async Task<Staff> CreateStaff(StaffRequest request, ApplicationUser currentUser)
        {
            var staff = StaffMapper.ToEntity(request);
            staff.Id = Guid.NewGuid();
            staff.CreatedBy = currentUser.Email;
            staff.CreatedOn = DateTime.UtcNow;
            staff.TenantId = currentUser.TenantId;
            staff.IsDeleted = false;

            await _staffRepository.AddAsync(staff);
            return staff;
        }
    }
}
