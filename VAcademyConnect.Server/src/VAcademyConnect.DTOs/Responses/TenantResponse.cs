using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Responses
{
    public class TenantResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public Status Status { get; set; }
        public string StatusName { get; set; }
        public TenantSubscriptionStatusResponse SubscriptionStatus { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? CreateBy { get; set; }
    }
}
