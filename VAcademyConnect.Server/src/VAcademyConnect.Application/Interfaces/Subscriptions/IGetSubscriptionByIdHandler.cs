using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Subscriptions
{
    public interface IGetSubscriptionByIdHandler
    {
        Task<AppResponse<SubscriptionResponse>> HandleAsync(Guid id);
    }
}
