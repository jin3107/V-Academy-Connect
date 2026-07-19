using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.PaymentChecks
{
    public interface ISearchPaymentCheckHandler
    {
        Task<AppResponse<SearchResponse<PaymentCheckResponse>>> HandleAsync(SearchRequest request);
    }
}
