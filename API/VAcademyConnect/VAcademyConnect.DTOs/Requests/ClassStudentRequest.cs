using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class ClassStudentRequest
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? ClassId { get; set; }

        [Required]
        public string? StudentId { get; set; }
    }
}
