using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.PaymentChecks
{
    public interface IGetPaymentCheckByIdHandler
    {
        Task<AppResponse<PaymentCheckResponse>> HandleAsync(Guid id);
    }
}
