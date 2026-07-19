using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.PaymentChecks
{
    public interface ICreatePaymentCheckHandler
    {
        Task<AppResponse<PaymentCheckResponse>> HandleAsync(PaymentCheckRequest request);
    }
}
