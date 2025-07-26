using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.Models.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public Status Status { get; set; }

        public TenantSubscription? TenantSubscription { get; set; }
        public ICollection<ApplicationUser>? ApplicationUsers { get; set; }
        public ICollection<Class>? Classes { get; set; }
        public ICollection<BackgroundJob>? BackgroundJobs { get; set; }
    }
}
