using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class TuitionFeeResponse
    {
        public Guid Id { get; set; }
        public string StudentId { get; set; }
        public string Month { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }

        
        public string? StudentName { get; set; }
        public string? Email { get; set; }
    }
}
