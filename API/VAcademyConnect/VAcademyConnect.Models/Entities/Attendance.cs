using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Models.Entities
{
    public class Attendance : BaseEntity
    {
        [ForeignKey("Class")]
        public Guid? ClassId { get; set; }
        public Class? Class { get; set; }

        [ForeignKey("Student")]
        public string? StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        public DateTime? Date { get; set; }
        public bool IsPresent { get; set; }
        public bool CountAsPaid { get; set; }
    }
}
