using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.Domain.Entities
{
    public class Subscription : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public int TrialDays { get; set; }

        public ICollection<TenantSubscription>? TenantSubscriptions { get; set; }
    }
}
