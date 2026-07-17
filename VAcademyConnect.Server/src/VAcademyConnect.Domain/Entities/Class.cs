using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.Domain.Entities
{
    public class Class : BaseEntity
    {
        public string Name { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public decimal Tuition { get; set; }

        public ICollection<ClassStudent>? ClassStudents { get; set; }
        public ICollection<Attendance>? Attendances { get; set; }
        public ICollection<LessonEvaluation>? LessonEvaluations { get; set; }
        public ICollection<Score>? Scores { get; set; }
    }
}
