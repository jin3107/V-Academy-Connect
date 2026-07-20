using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class TenantMapper
    {
        public static TenantRequest ToRequest(Tenant entity)
        {
            return new TenantRequest
            {
                Name = entity.Name,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                Status = entity.Status,
                Type = entity.Type
            };
        }

        public static Tenant ToEntity(TenantRequest request)
        {
            return new Tenant
            {
                Name = request.Name,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = request.Status,
                Type = request.Type
            };
        }

        public static TenantResponse ToResponse(Tenant entity)
        {
            var now = DateTime.UtcNow;
            bool isExpired = false;
            bool isExpiringSoon = false;
            DateTime? expirationDate = null;
            string? planName = null;
            bool hasActiveSubscription = false;
            bool isFreeplan = false;

            if (entity.TenantSubscription != null)
            {
                expirationDate = entity.TenantSubscription.EndDate;
                planName = entity.TenantSubscription.Subscription?.Name;
                isFreeplan = planName?.ToLower() == "free";
                
                if (expirationDate.HasValue)
                {
                    isExpired = expirationDate.Value < now;
                    isExpiringSoon = !isExpired && expirationDate.Value <= now.AddDays(7);
                    hasActiveSubscription = !isExpired;
                }
                else
                {
                    hasActiveSubscription = true;
                    isExpired = false;
                    isExpiringSoon = false;
                }
            }

            
            var response = new TenantResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                Status = entity.Status,
                StatusName = entity.Status.ToString(),
                Type = entity.Type,
                TypeName = entity.Type.ToString(),
                SubscriptionStatus = new TenantSubscriptionStatusResponse
                {
                    HasActiveSubscription = hasActiveSubscription,
                    IsExpiringSoon = isExpiringSoon,
                    IsExpired = isExpired,
                    ExpirationDate = expirationDate,
                    SubscriptionPlanName = planName ?? "No Subscription plan",
                },
                CreatedOn = entity.CreatedOn,
                CreateBy = entity.CreatedBy
            };
            return response;
        }
    }
}
