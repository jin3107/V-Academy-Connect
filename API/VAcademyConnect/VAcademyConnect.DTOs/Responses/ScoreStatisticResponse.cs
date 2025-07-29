using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class ScoreStatisticResponse : BaseDto
    {
        public string? StudentId { get; set; }
        public string Month { get; set; }
        public decimal AvarageScore { get; set; }

        public string? StudentName { get; set; }
    }
}
