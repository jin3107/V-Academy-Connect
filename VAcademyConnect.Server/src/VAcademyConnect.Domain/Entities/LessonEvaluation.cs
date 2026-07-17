using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Domain.Entities
{
    public class LessonEvaluation : BaseEntity
    {
        [ForeignKey("Class")]
        public Guid? ClassId { get; set; }
        public Class? Class { get; set; }

        [ForeignKey("Student")]
        public string? StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        public DateTime? Date { get; set; }
        public string? Comment { get; set; }
        public int? Rating { get; set; }
    }
}
