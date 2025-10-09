using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class TenantSubscriptionRequest : BaseDto
    {
        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid SubscriptionId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsTrial { get; set; }
    }
}
