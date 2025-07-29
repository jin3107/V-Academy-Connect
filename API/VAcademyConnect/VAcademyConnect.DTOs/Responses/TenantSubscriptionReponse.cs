using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class TenantSubscriptionResponse : BaseDto
    {
        public Guid TenantId { get; set; }
        public string? TenantName { get; set; }

        public Guid SubscriptionId { get; set; }
        public string? SubscriptionName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsTrial { get; set; }
    }
}
