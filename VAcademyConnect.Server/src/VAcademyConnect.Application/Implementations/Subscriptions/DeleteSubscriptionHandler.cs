using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Subscriptions;

namespace VAcademyConnect.Application.Implementations.Subscriptions
{
    public class DeleteSubscriptionHandler : IDeleteSubscriptionHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteSubscriptionHandler(ISubscriptionRepository subscriptionRepository, ICurrentUserService currentUserService)
        {
            _subscriptionRepository = subscriptionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<string>> HandleAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var subscription = await _subscriptionRepository.GetAsync(id);
                if (subscription == null || subscription.IsDeleted == true)
                    return result.BuildError("Subscription was not found or has been deleted.");
                subscription.IsDeleted = true;
                subscription.ModifiedBy = user.Email;

                await _subscriptionRepository.EditAsync(subscription);
                result.BuildResult("Subscription has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
