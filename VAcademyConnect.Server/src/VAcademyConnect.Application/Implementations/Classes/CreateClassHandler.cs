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

        public CreateClassHandler(ICurrentUserService currentUserService, IClassRepository classRepository,
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

                var newClass = ClassMapper.ToEntity(request);
                newClass.Id = Guid.NewGuid();
                newClass.Name = request.Name;
                newClass.TenantId = request.TenantId;
                newClass.Tuition = request.Tuition;
                newClass.CreatedBy = user.Email;
                newClass.CreatedOn = DateTime.UtcNow;

                await _classRepository.AddAsync(newClass);
                var response = ClassMapper.ToResponse(newClass);
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
