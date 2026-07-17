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
                TenantName = entity.Tenant?.Name,
                SubscriptionId = entity.SubscriptionId,
                SubscriptionName = entity.Subscription?.Name,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsTrial = entity.IsTrial,
                CreatedOn = entity.CreatedOn,
                CreateBy = entity.CreatedBy
            };
        }

        // Tạo trial subscription Standard cho tenant mới (30 ngày miễn phí)
        public static TenantSubscription ToTrialSubscription(Guid tenantId, Subscription standardPlan)
        {
            return new TenantSubscription
            {
                TenantId = tenantId,
                SubscriptionId = standardPlan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                IsTrial = true
            };
        }

        // Tạo subscription Free khi Standard hết hạn (ko có EndDate)
        public static TenantSubscription ToFreeSubscription(Guid tenantId, Subscription freePlan)
        {
            return new TenantSubscription
            {
                TenantId = tenantId,
                SubscriptionId = freePlan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                IsTrial = false
            };
        }

        // Gia hạn subscription Standard (ko phải trial)
        public static TenantSubscription ToRenewedStandardSubscription(Guid tenantId, Subscription standardPlan, 
            string createdBy, int months = 1)
        {
            return new TenantSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                SubscriptionId = standardPlan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(months),
                IsTrial = false,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
