using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Payments
{
    public interface ISearchPaymentHandler
    {
        Task<AppResponse<SearchResponse<PaymentResponse>>> HandleAsync(SearchRequest request);
    }
}
