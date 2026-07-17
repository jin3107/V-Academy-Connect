using System.ComponentModel.DataAnnotations;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Authentication.Requests
{
    public class SendOtpRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Purpose is required.")]
        public OtpPurpose Purpose { get; set; }
    }
}
