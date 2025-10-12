using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public string? StudentId { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime? TransferTime { get; set; }
        public string? QRInfo { get; set; }

        public string? StudentName { get; set; }
    }
}
