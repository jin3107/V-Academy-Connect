using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Requests
{
    public class TuitionFeeRequest
    {
        [Required(ErrorMessage = "StudentId is required.")]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        public decimal Amount { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
