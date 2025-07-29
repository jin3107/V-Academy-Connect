using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class ScoreStatisticRequest : BaseDto
    {
        [Required(ErrorMessage = "StudentId is required.")]
        public string? StudentId { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Average score is required.")]
        [Range(0, 10, ErrorMessage = "Score must be between 0 and 10.")]
        public decimal AvarageScore { get; set; }
    }

}
