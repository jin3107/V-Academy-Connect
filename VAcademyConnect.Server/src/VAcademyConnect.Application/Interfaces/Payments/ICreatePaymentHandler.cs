using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Payments
{
    public interface ICreatePaymentHandler
    {
        Task<AppResponse<PaymentResponse>> HandleAsync(PaymentRequest request);
    }
}
