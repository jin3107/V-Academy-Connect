using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public Role? Role { get; set; }

        [InverseProperty("Student")]
        public ICollection<Attendance>? Attendances { get; set; }

        [InverseProperty("Student")]
        public ICollection<ClassStudent>? ClassStudents { get; set; }

        [InverseProperty("Student")]
        public ICollection<LessonEvaluation>? LessonEvaluations { get; set; }

        [InverseProperty("Student")]
        public ICollection<Score>? Scores { get; set; }

        [InverseProperty("Student")]
        public ICollection<TuitionFee>? TuitionFees { get; set; }

        [InverseProperty("Student")]
        public ICollection<Payment>? Payments { get; set; }

        [InverseProperty("CheckedByUser")]
        public ICollection<PaymentCheck>? PaymentChecks { get; set; }

        [InverseProperty("Teacher")]
        public ICollection<Class>? Classes { get; set; }

        [InverseProperty("Student")]
        public ICollection<ScoreStatistic>? ScoreStatistics { get; set; }
    }
}
