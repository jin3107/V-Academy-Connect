using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Services.Mapping
{
    public static class SubscriptionMapper
    {
        public static SubscriptionRequest ToRequest(Subscription entity)
        {
            return new SubscriptionRequest
            {
                Name = entity.Name,
                Price = entity.Price,
                BillingCycle = entity.BillingCycle,
                TrialDays = entity.TrialDays
            };
        }

        public static Subscription ToEntity(SubscriptionRequest request)
        {
            return new Subscription
            {
                Name = request.Name,
                Price = request.Price,
                BillingCycle = request.BillingCycle,
                TrialDays = request.TrialDays
            };
        }

        public static SubscriptionResponse ToResponse(Subscription entity)
        {
            return new SubscriptionResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                BillingCycle = entity.BillingCycle,
                BillingCycleName = entity.BillingCycle.ToString(),
                TrialDays = entity.TrialDays,
                CreateBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
            };
        }
    }
}
