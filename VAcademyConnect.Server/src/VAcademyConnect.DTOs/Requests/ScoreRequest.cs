using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class ScoreRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "StudentId is required.")]
        public string? StudentId { get; set; }

        [Required(ErrorMessage = "ClassId is required.")]
        public Guid? ClassId { get; set; }

        [Required(ErrorMessage = "TestName is required.")]
        public string TestName { get; set; }

        [Required(ErrorMessage = "Mark is required.")]
        [Range(0, 10, ErrorMessage = "Mark must be between 0 and 10.")]
        public decimal Mark { get; set; }
    }
}
