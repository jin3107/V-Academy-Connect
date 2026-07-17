using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Subscriptions;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Subscriptions
{
    public class GetSubscriptionByIdHandler : IGetSubscriptionByIdHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetSubscriptionByIdHandler(ISubscriptionRepository subscriptionRepository, ICurrentUserService currentUserService)
        {
            _subscriptionRepository = subscriptionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SubscriptionResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<SubscriptionResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var subscription = await _subscriptionRepository.FindByAsync(sub => sub.Id == id).FirstOrDefaultAsync();
                if (subscription == null || subscription.IsDeleted == true)
                    return result.BuildError("Subscription was not found or has been deleted.");

                var response = SubscriptionMapper.ToResponse(subscription);
                result.BuildResult(response, "Subscription has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
