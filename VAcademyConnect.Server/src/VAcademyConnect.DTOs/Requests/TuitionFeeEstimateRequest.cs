using System;
using System.ComponentModel.DataAnnotations;

namespace VAcademyConnect.DTOs.Requests
{
    public class TuitionFeeEstimateRequest
    {
        [Required(ErrorMessage = "StudentId is required.")]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "ClassId is required.")]
        public Guid? ClassId { get; set; }

        [Required(ErrorMessage = "CycleStartDate is required.")]
        public DateTime CycleStartDate { get; set; }
    }
}
