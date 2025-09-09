using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Responses
{
    public class TenantResponse : BaseDto
    {
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public Status Status { get; set; }

        public string StatusName { get; set; }

        public Guid? TenantSubscriptionId { get; set; }
        public string? SubscriptionName { get; set; }
        public DateTime? SubscribedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
}
