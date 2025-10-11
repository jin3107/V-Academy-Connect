using System;

namespace VAcademyConnect.DTOs.Responses
{
    public class TenantSubscriptionStatusResponse
    {
        public bool HasActiveSubscription { get; set; }
        public bool IsExpiringSoon { get; set; }
        public bool IsExpired { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string SubscriptionPlanName { get; set; }
    }
}