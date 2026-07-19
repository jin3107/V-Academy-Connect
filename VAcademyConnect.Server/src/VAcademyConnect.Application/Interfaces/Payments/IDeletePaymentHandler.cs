using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.Payments
{
    public interface IDeletePaymentHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
