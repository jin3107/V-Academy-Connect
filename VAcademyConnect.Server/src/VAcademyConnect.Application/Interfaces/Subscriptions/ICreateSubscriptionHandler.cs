using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Subscriptions
{
    public interface ICreateSubscriptionHandler
    {
        Task<AppResponse<SubscriptionResponse>> HandleAsync(SubscriptionRequest request);
    }
}
