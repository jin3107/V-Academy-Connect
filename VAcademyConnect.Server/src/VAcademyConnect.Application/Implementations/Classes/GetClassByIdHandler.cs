using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Classes;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Classes
{
    public class GetClassByIdHandler : IGetClassByIdHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IClassRepository _classRepository;
        private readonly ITenantRepository _tenantRepository;

        public GetClassByIdHandler(ICurrentUserService currentUserService, IClassRepository classRepository,
            ITenantRepository tenantRepository)
        {
            _currentUserService = currentUserService;
            _classRepository = classRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<AppResponse<ClassResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<ClassResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unthorize.");

                var classes = await _classRepository.FindByAsync(c => c.Id == id)
                    .Include(c => c.ClassStudents)!.ThenInclude(cs => cs.Student).FirstOrDefaultAsync();
                if (classes == null || classes.IsDeleted == true)
                    return result.BuildError("Class was not found or has been deleted.");

                var response = ClassMapper.ToResponse(classes);
                if (response.TenantId.HasValue)
                {
                    var tenantName = _tenantRepository.FindByAsync(c => c.Id == classes.TenantId).FirstOrDefault();
                    response.TenantName = tenantName?.Name ?? "None";
                }
                result.BuildResult(response, "Class has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
