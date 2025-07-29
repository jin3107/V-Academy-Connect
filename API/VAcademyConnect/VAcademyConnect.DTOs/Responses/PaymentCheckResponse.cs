using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class PaymentCheckResponse : BaseDto
    {
        public Guid? PaymentId { get; set; }
        public string? CheckedBy { get; set; }
        public DateTime? CheckTime { get; set; }
        public string? Note { get; set; }

        public string? CheckedByName { get; set; }
    }
}
