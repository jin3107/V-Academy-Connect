using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Tenants
{
    public interface ISearchTenantHandler
    {
        Task<AppResponse<SearchResponse<TenantResponse>>> HandleAsync(SearchRequest request);
    }
}
