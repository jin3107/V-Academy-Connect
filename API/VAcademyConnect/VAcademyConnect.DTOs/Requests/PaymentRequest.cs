using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class PaymentRequest : BaseDto
    {
        [Required(ErrorMessage = "StudentId is required.")]
        public string? StudentId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Transfer time is required.")]
        public DateTime? TransferTime { get; set; }

        public string? QRInfo { get; set; }
    }
}
