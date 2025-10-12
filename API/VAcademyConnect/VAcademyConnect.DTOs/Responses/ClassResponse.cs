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
        public string? TeacherId { get; set; }
        public Guid? TenantId { get; set; }
        public decimal Tuition { get; set; }

        public string? TeacherName { get; set; }
        public string? TenantName { get; set; }

        public List<StudentInClassResponse>? Students { get; set; }
    }
}
