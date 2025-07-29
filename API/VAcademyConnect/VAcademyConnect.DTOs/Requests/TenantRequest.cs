using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Requests
{
    public class TenantRequest : BaseDto
    {
        [Required(ErrorMessage = "Tenant name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Contact phone is required.")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public Status Status { get; set; }
    }
}
