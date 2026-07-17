using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Tenants
{
    public interface IRenewTenantStandardSubscriptionHandler
    {
        Task<AppResponse<TenantResponse>> HandleAsync(Guid tenantId);
    }
}
