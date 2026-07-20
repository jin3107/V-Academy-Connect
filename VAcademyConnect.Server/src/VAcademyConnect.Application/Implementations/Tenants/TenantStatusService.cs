using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.Tenants
{
    public class TenantStatusService : ITenantStatusService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;

        public TenantStatusService(ISubscriptionRepository subscriptionRepository,
            ITenantSubscriptionRepository tenantSubscriptionRepository, ITenantRepository tenantRepository,
            ICurrentUserService currentUserService)
        {
            _subscriptionRepository = subscriptionRepository;
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> CheckAndUpdateExpiredSubscriptionAsync(Tenant tenant, bool saveChanges = true)
        {
            try
            {
                bool statusChanged = false;

                if (tenant.TenantSubscription?.EndDate < DateTime.UtcNow && tenant.TenantSubscription?.EndDate != null)
                {
                    var freePlan = await _subscriptionRepository.FindByAsync(s => s.Name == "Free" && !s.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (freePlan != null)
                    {
                        var oldSubscription = await _tenantSubscriptionRepository.GetAsync(tenant.TenantSubscription.Id);
                        if (oldSubscription != null)
                        {
                            oldSubscription.SubscriptionId = freePlan.Id;
                            oldSubscription.Subscription = freePlan;
                            oldSubscription.IsTrial = false;
                            oldSubscription.StartDate = DateTime.UtcNow;
                            oldSubscription.EndDate = null;
                            oldSubscription.ModifiedOn = DateTime.UtcNow;
                            oldSubscription.ModifiedBy = (await _currentUserService.GetCurrentUserAsync())?.Email ?? "System";

                            await _tenantSubscriptionRepository.EditAsync(oldSubscription);
                        }

                        tenant.Status = Status.Active;
                        tenant.ModifiedOn = DateTime.UtcNow;
                        tenant.ModifiedBy = "System - Switched to Free Plan";
                        statusChanged = true;

                        if (saveChanges)
                        {
                            await _tenantRepository.EditAsync(tenant);
                        }
                    }
                }
                return statusChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
            }
            return false;
        }
    }
}
