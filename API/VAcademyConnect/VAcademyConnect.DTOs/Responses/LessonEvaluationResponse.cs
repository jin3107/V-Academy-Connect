using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class LessonEvaluationResponse : BaseDto
    {
        public Guid? ClassId { get; set; }
        public string? StudentId { get; set; }
        public DateTime? Date { get; set; }
        public string? Comment { get; set; }
        public int? Rate { get; set; }

        public string? StudentName { get; set; }
        public string? ClassName { get; set; }
    }
}
