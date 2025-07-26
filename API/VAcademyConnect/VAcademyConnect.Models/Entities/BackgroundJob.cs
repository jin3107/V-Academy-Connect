using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Models.Entities
{
    public class BackgroundJob : BaseEntity
    {
        public DateTime RunDate { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public string? Result { get; set; }
        public string? LogDetail { get; set; }
    }
}
