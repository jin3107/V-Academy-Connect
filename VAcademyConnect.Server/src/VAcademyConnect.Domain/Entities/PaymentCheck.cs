using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Domain.Entities
{
    public class PaymentCheck : BaseEntity
    {
        [ForeignKey("Payment")]
        public Guid? PaymentId { get; set; }
        public Payment? Payment { get; set; }

        [ForeignKey("CheckedByUser")]
        public string? CheckedBy { get; set; }
        public ApplicationUser? CheckedByUser { get; set; }

        public DateTime? CheckTime { get; set; }
        public string? Note { get; set; }
    }
}
