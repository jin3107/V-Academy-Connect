using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Subscriptions;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Subscriptions
{
    public class CreateSubscriptionHandler : ICreateSubscriptionHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateSubscriptionHandler(ISubscriptionRepository subscriptionRepository, ICurrentUserService currentUserService)
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
                    return result.BuildError("Unauthorize.");

                var newSub = SubscriptionMapper.ToEntity(request);
                newSub.Id = Guid.NewGuid();
                newSub.Name = request.Name;
                newSub.Price = request.Price;
                newSub.BillingCycle = request.BillingCycle;
                newSub.TrialDays = request.TrialDays;
                newSub.CreatedOn = DateTime.UtcNow;
                newSub.CreatedBy = user.Email;

                await _subscriptionRepository.AddAsync(newSub);
                var response = SubscriptionMapper.ToResponse(newSub);
                result.BuildResult(response, "Subscription has been created successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
