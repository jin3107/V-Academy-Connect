using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Domain.Entities
{
    public class Score : BaseEntity
    {
        [ForeignKey("Student")]
        public string? StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        [ForeignKey("Class")]
        public Guid? ClassId { get; set; }
        public Class? Class { get; set; }
        public string TestName { get; set; }
        public decimal Mark { get; set; }
    }
}
