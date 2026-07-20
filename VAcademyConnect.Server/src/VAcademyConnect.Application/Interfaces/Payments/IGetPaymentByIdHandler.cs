using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Payments
{
    public interface IGetPaymentByIdHandler
    {
        Task<AppResponse<PaymentResponse>> HandleAsync(Guid id);
    }
}
