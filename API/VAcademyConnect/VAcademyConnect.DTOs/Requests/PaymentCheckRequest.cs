using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class PaymentCheckRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "PaymentId is required.")]
        public Guid? PaymentId { get; set; }

        [Required(ErrorMessage = "CheckedBy (UserId) is required.")]
        public string? CheckedBy { get; set; }

        [Required(ErrorMessage = "CheckTime is required.")]
        public DateTime? CheckTime { get; set; }

        public string? Note { get; set; }
    }
}
