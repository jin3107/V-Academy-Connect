using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class BackgroundJobRequest : BaseDto
    {
        [Required]
        public DateTime RunDate { get; set; }

        [Required]
        public Guid? TenantId { get; set; }

        public string? Result { get; set; }
        public string? LogDetail { get; set; }
    }
}
