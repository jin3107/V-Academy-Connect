using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Responses
{
    public class StaffResponse : BaseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }

        public string? Address { get; set; }
        public Genders? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreateBy { get; set; }

        public string? Password { get; set; }
    }
}
