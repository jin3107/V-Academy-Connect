using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class LessonEvaluationRequest : BaseDto
    {
        [Required]
        public Guid? ClassId { get; set; }

        [Required]
        public string? StudentId { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        public string? Comment { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }
    }
}
