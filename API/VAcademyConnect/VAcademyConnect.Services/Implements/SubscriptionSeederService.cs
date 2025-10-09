using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.Models.Entities;
using VAcademyConnect.Repositories.Interfaces;

namespace VAcademyConnect.Services.Implements
{
    public class SubscriptionSeederService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionSeederService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task SeedDefaultSubscriptionsAsync()
        {
            // Ki?m tra xem ?ã có subscription plans ch?a
            var existingPlans = await _subscriptionRepository.FindByAsync(s => !s.IsDeleted).ToListAsync();
            
            if (!existingPlans.Any())
            {
                var subscriptions = new List<Subscription>
                {
                    // Free Plan
                    new Subscription
                    {
                        Id = Guid.NewGuid(),
                        Name = "Free",
                        Price = 0,
                        BillingCycle = BillingCycle.None,
                        TrialDays = 0,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = "System",
                        IsDeleted = false
                    },
                    
                    // Standard Plan
                    new Subscription
                    {
                        Id = Guid.NewGuid(),
                        Name = "Standard",
                        Price = 100000, // 100,000 VND
                        BillingCycle = BillingCycle.Monthly,
                        TrialDays = 30, // 30 ngày trial cho user m?i
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = "System",
                        IsDeleted = false
                    }
                };

                await _subscriptionRepository.AddRangeAsync(subscriptions);
            }
        }
    }
}