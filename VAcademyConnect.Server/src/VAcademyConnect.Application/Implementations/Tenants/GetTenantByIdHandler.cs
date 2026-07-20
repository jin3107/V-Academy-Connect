using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Tenants
{
    public class GetTenantByIdHandler : IGetTenantByIdHandler
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITenantStatusService _tenantStatusService;

        public GetTenantByIdHandler(ITenantRepository tenantRepository, ICurrentUserService currentUserService,
            ITenantStatusService tenantStatusService)
        {
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
            _tenantStatusService = tenantStatusService;
        }

        public async Task<AppResponse<TenantResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var tenant = await _tenantRepository.FindByAsync(sub => sub.Id == id)
                    .Include(t => t.TenantSubscription)
                    .ThenInclude(ts => ts!.Subscription)
                    .FirstOrDefaultAsync();

                if (tenant == null || tenant.IsDeleted == true)
                    return result.BuildError("Tenant was not found or has been deleted.");

                var statusChanged = await _tenantStatusService.CheckAndUpdateExpiredSubscriptionAsync(tenant);

                if (statusChanged)
                    tenant = await _tenantRepository.FindByAsync(sub => sub.Id == id)
                        .Include(t => t.TenantSubscription).ThenInclude(ts => ts!.Subscription).FirstOrDefaultAsync();

                var response = TenantMapper.ToResponse(tenant!);
                var message = "Tenant has been found.";
                if (statusChanged)
                    message += " Tenant has been switched to Free plan due to expired subscription.";
                result.BuildResult(response, message);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
