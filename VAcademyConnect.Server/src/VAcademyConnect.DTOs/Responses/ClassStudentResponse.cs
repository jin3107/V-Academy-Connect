using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class ClassStudentResponse
    {
        public Guid Id { get; set; }
        public Guid? ClassId { get; set; }
        public string? StudentId { get; set; }

        public string? StudentName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
