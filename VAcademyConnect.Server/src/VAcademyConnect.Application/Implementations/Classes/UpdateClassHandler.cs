using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Classes;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Classes
{
    public class UpdateClassHandler : IUpdateClassHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IClassRepository _classRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IStaffRepository _staffRepository;

        public UpdateClassHandler(ICurrentUserService currentUserService, IClassRepository classRepository,
            ITenantRepository tenantRepository, IStaffRepository staffRepository)
        {
            _currentUserService = currentUserService;
            _classRepository = classRepository;
            _tenantRepository = tenantRepository;
            _staffRepository = staffRepository;
        }

        public async Task<AppResponse<ClassResponse>> HandleAsync(ClassRequest request)
        {
            var result = new AppResponse<ClassResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var classes = await _classRepository.GetAsync(request.Id);
                if (classes == null || classes.IsDeleted == true)
                    return result.BuildError("Class was not found or has been deleted.");

                if (request.TeacherId.HasValue)
                {
                    var teacher = await _staffRepository.GetAsync(request.TeacherId.Value);
                    if (teacher == null || teacher.IsDeleted || !teacher.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                        return result.BuildError("TeacherId must reference an existing Staff with Teacher role.");
                    if (teacher.TenantId != request.TenantId)
                        return result.BuildError("Teacher must belong to the same tenant as the class.");
                }

                classes.Name = request.Name;
                classes.TenantId = request.TenantId;
                classes.Tuition = request.Tuition;
                classes.TeacherId = request.TeacherId;
                classes.TeachingDayOfWeek = request.TeachingDayOfWeek;
                classes.TeachingStartTime = request.TeachingStartTime;
                classes.TeachingEndTime = request.TeachingEndTime;
                classes.ModifiedBy = user.Email;
                classes.ModifiedOn = DateTime.UtcNow;

                await _classRepository.EditAsync(classes);

                var updatedEntity = await _classRepository.FindByAsync(c => c.Id == classes.Id)
                    .Include(c => c.Teacher).FirstOrDefaultAsync();
                var response = ClassMapper.ToResponse(updatedEntity ?? classes);
                if (response.TenantId.HasValue)
                {
                    var tenantName = _tenantRepository.FindByAsync(c => c.Id == classes.TenantId).FirstOrDefault();
                    response.TenantName = tenantName?.Name ?? "None";
                }
                result.BuildResult(response, "Class has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
