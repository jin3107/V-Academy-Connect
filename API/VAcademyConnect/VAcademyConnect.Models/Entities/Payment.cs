using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Models.Entities
{
    public class Payment : BaseEntity
    {
        [ForeignKey("Student")]
        public string? StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime? TransferTime { get; set; }
        public string? QRInfo { get; set; }
        public ICollection<PaymentCheck>? PaymentChecks { get; set; }
    }
}
