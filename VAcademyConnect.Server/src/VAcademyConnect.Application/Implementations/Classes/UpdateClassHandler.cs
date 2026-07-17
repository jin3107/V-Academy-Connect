using MayNghien.Infrastructures.Models.Responses;
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

        public UpdateClassHandler(ICurrentUserService currentUserService, IClassRepository classRepository,
            ITenantRepository tenantRepository)
        {
            _currentUserService = currentUserService;
            _classRepository = classRepository;
            _tenantRepository = tenantRepository;
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

                classes.Name = request.Name;
                classes.TenantId = request.TenantId;
                classes.Tuition = request.Tuition;
                classes.ModifiedBy = user.Email;
                classes.ModifiedOn = DateTime.UtcNow;

                await _classRepository.EditAsync(classes);
                var response = ClassMapper.ToResponse(classes);
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
