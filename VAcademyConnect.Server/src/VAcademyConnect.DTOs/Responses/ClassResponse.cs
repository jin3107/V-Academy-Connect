using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class ClassResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? TenantId { get; set; }
        public decimal Tuition { get; set; }
        public string? TenantName { get; set; }

        public Guid? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public DayOfWeek? TeachingDayOfWeek { get; set; }
        public TimeSpan? TeachingStartTime { get; set; }
        public TimeSpan? TeachingEndTime { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }

        public List<ClassStudentResponse>? Students { get; set; }
    }
}
