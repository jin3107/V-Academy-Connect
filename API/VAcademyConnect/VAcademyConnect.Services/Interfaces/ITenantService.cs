using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Services.Interfaces
{
    public interface ITenantService
    {
        Task<AppResponse<TenantResponse>> GetByIdAsync(Guid id);
        Task<AppResponse<TenantResponse>> UpdateAsync(TenantRequest request);
        Task<AppResponse<SearchResponse<TenantResponse>>> SearchAsync(SearchRequest request);
        
        // Gia hạn subscription Standard cho tenant (chỉ admin)
        Task<AppResponse<TenantResponse>> RenewStandardSubscriptionAsync(Guid tenantId, int months = 1);
    }
}
