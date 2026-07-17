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
    public class CreateClassHandler : ICreateClassHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IClassRepository _classRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IStaffRepository _staffRepository;

        public CreateClassHandler(ICurrentUserService currentUserService, IClassRepository classRepository,
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

                if (request.TeacherId.HasValue)
                {
                    var teacher = await _staffRepository.GetAsync(request.TeacherId.Value);
                    if (teacher == null || teacher.IsDeleted || !teacher.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                        return result.BuildError("TeacherId must reference an existing Staff with Teacher role.");
                    if (teacher.TenantId != request.TenantId)
                        return result.BuildError("Teacher must belong to the same tenant as the class.");
                }

                var newClass = ClassMapper.ToEntity(request);
                newClass.Id = Guid.NewGuid();
                newClass.Name = request.Name;
                newClass.TenantId = request.TenantId;
                newClass.Tuition = request.Tuition;
                newClass.CreatedBy = user.Email;
                newClass.CreatedOn = DateTime.UtcNow;

                await _classRepository.AddAsync(newClass);

                var createdEntity = await _classRepository.FindByAsync(c => c.Id == newClass.Id)
                    .Include(c => c.Teacher).FirstOrDefaultAsync();
                var response = ClassMapper.ToResponse(createdEntity ?? newClass);
                if (request.TenantId.HasValue)
                {
                    var tenantName = _tenantRepository.FindByAsync(c => c.Id == newClass.TenantId).FirstOrDefault();
                    response.TenantName = tenantName?.Name ?? "None";
                }
                result.BuildResult(response, "Class has been created successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
