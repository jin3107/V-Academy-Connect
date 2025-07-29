using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class ClassRequest : BaseDto
    {
        [Required(ErrorMessage = "Class name is required.")]
        public string Name { get; set; }

        [Required]
        public string? TeacherId { get; set; }

        [Required]
        public Guid? TenantId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tuition must be a positive number.")]
        public decimal Tuition { get; set; }
    }
}
