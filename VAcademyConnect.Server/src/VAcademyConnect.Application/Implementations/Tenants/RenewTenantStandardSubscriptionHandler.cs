using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Tenants
{
    public class RenewTenantStandardSubscriptionHandler : IRenewTenantStandardSubscriptionHandler
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly ICurrentUserService _currentUserService;

        public RenewTenantStandardSubscriptionHandler(ITenantRepository tenantRepository,
            ISubscriptionRepository subscriptionRepository, ITenantSubscriptionRepository tenantSubscriptionRepository,
            ICurrentUserService currentUserService)
        {
            _tenantRepository = tenantRepository;
            _subscriptionRepository = subscriptionRepository;
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<TenantResponse>> HandleAsync(Guid tenantId)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorized");

                var roles = await _currentUserService.GetRolesAsync(user);
                if (!roles.Any(r => r == "SuperAdmin" || r == "Admin"))
                    return result.BuildError("Only administrators can renew subscriptions.");

                var standardPlan = await _subscriptionRepository
                    .FindByAsync(s => s.Name == "Standard" && !s.IsDeleted)
                    .FirstOrDefaultAsync();

                if (standardPlan == null)
                    return result.BuildError("Standard subscription plan not found.");

                var currentTenantSubscription = await _tenantSubscriptionRepository
                    .FindByAsync(s => s.TenantId == tenantId && !s.IsDeleted)
                    .FirstOrDefaultAsync();

                if (currentTenantSubscription == null)
                    return result.BuildError("No active subscription found for this tenant.");

                currentTenantSubscription.SubscriptionId = standardPlan.Id;
                currentTenantSubscription.StartDate = DateTime.UtcNow;
                currentTenantSubscription.EndDate = DateTime.UtcNow.AddMonths(1);
                currentTenantSubscription.IsTrial = false;
                currentTenantSubscription.ModifiedOn = DateTime.UtcNow;
                currentTenantSubscription.ModifiedBy = user.Email;
                await _tenantSubscriptionRepository.EditAsync(currentTenantSubscription);

                var tenant = await _tenantRepository.FindByAsync(t => t.Id == tenantId)
                    .FirstOrDefaultAsync();

                if (tenant != null)
                {
                    tenant.Status = Status.Active;
                    tenant.ModifiedBy = user.Email;
                    tenant.ModifiedOn = DateTime.UtcNow;
                    await _tenantRepository.EditAsync(tenant);
                }

                tenant = await _tenantRepository.FindByAsync(t => t.Id == tenantId)
                    .Include(t => t.TenantSubscription)
                    .ThenInclude(ts => ts!.Subscription)
                    .FirstOrDefaultAsync();

                var response = TenantMapper.ToResponse(tenant!);
                result.BuildResult(response, "Subscription renewed successfully for 1 month(s). Tenant is now active.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
