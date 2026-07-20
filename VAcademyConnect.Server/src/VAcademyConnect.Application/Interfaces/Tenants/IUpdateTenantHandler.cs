using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Tenants
{
    public interface IUpdateTenantHandler
    {
        Task<AppResponse<TenantResponse>> HandleAsync(TenantRequest request);
    }
}
