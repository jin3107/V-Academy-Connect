using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Responses
{
    public class SubscriptionResponse : BaseDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public int TrialDays { get; set; }

        // Cần hiển thị Enum dưới dạng text
        public string BillingCycleName => BillingCycle.ToString();
    }
}
