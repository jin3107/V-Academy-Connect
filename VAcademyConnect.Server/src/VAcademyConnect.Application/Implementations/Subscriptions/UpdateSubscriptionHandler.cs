using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Subscriptions;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Subscriptions
{
    public class UpdateSubscriptionHandler : IUpdateSubscriptionHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSubscriptionHandler(ISubscriptionRepository subscriptionRepository, ICurrentUserService currentUserService)
        {
            _subscriptionRepository = subscriptionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SubscriptionResponse>> HandleAsync(SubscriptionRequest request)
        {
            var result = new AppResponse<SubscriptionResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var subscription = await _subscriptionRepository.GetAsync(request.Id);
                if (subscription == null || subscription.IsDeleted == true)
                    return result.BuildError("Subscription was not found or has been deleted.");

                subscription.Name = request.Name;
                subscription.Price = request.Price;
                subscription.TrialDays = request.TrialDays;
                subscription.BillingCycle = request.BillingCycle;
                subscription.ModifiedBy = user.Email;
                subscription.ModifiedOn = DateTime.UtcNow;

                await _subscriptionRepository.EditAsync(subscription);
                var response = SubscriptionMapper.ToResponse(subscription);
                result.BuildResult(response, "Subscription has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
