using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Responses
{
    public class SubscriptionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int TrialDays { get; set; }

        public BillingCycle BillingCycle { get; set; }
        public string BillingCycleName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreateBy { get; set; }
    }
}
