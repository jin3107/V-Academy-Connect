using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Requests
{
    public class SubscriptionRequest : BaseDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Billing cycle is required.")]
        public BillingCycle BillingCycle { get; set; }

        [Required(ErrorMessage = "Trial days are required.")]
        [Range(0, 365, ErrorMessage = "TrialDays must be between 0 and 365.")]
        public int TrialDays { get; set; }
    }
}
