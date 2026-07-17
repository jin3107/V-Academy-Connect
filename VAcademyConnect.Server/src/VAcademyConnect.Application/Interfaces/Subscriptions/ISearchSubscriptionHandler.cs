using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Subscriptions
{
    public interface ISearchSubscriptionHandler
    {
        Task<AppResponse<SearchResponse<SubscriptionResponse>>> HandleAsync(SearchRequest request);
    }
}
