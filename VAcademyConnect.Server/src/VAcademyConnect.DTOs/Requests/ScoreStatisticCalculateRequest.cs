using System;
using System.ComponentModel.DataAnnotations;

namespace VAcademyConnect.DTOs.Requests
{
    public class ScoreStatisticCalculateRequest
    {
        [Required(ErrorMessage = "Month is required.")]
        [RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])$", ErrorMessage = "Month must be in yyyy-MM format.")]
        public string Month { get; set; }

        public Guid? ClassId { get; set; }
    }
}
