using System.ComponentModel.DataAnnotations;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Authentication.Requests
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Purpose is required.")]
        public OtpPurpose Purpose { get; set; }
    }
}
