using System.ComponentModel.DataAnnotations;

namespace VAcademyConnect.DTOs.Requests
{
    public class StudentRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public Guid? TenantId { get; set; }
    }
}
