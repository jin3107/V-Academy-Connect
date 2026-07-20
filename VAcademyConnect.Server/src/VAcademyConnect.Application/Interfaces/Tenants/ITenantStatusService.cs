using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Interfaces.Tenants
{
    public interface ITenantStatusService
    {
        Task<bool> CheckAndUpdateExpiredSubscriptionAsync(Tenant tenant, bool saveChanges = true);
    }
}
