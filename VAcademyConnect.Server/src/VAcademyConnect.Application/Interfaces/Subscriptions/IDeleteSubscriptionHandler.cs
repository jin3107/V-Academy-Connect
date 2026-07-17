using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.Subscriptions
{
    public interface IDeleteSubscriptionHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
