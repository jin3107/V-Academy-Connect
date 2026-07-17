using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Domain.Entities
{
    public class TuitionFee : BaseEntity
    {
        [ForeignKey("Student")]
        public string? StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        public string Month { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
