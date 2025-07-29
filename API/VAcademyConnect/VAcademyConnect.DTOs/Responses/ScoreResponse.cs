using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class ScoreResponse : BaseDto
    {
        public string? StudentId { get; set; }
        public Guid? ClassId { get; set; }
        public string TestName { get; set; }
        public decimal Mark { get; set; }

        public string? StudentName { get; set; }
        public string? ClassName { get; set; }
    }
}
