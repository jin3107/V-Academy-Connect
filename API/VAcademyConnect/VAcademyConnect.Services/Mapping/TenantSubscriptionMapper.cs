using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Models.Entities;

namespace VAcademyConnect.Services.Mapping
{
    public static class TenantSubscriptionMapper
    {
        public static TenantSubscriptionRequest ToRequest(TenantSubscription entity)
        {
            return new TenantSubscriptionRequest
            {
                TenantId = entity.TenantId,
                SubscriptionId = entity.SubscriptionId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsTrial = entity.IsTrial
            };
        }

        public static TenantSubscription ToEntity(TenantSubscriptionRequest request)
        {
            return new TenantSubscription
            {
                TenantId = request.TenantId,
                SubscriptionId = request.SubscriptionId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsTrial = request.IsTrial
            };
        }

        public static TenantSubscriptionResponse ToResponse(TenantSubscription entity)
        {
            return new TenantSubscriptionResponse
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                SubscriptionId = entity.SubscriptionId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsTrial = entity.IsTrial
            };
        }

        public static TenantSubscription ToTrialSubscription(Guid tenantId, Subscription trialPlan)
        {
            return new TenantSubscription
            {
                TenantId = tenantId,
                SubscriptionId = trialPlan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(trialPlan.TrialDays),
                IsTrial = true
            };
        }
    }
}
