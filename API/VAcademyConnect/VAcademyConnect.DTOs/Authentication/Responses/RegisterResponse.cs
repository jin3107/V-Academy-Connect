using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Authentication.Responses
{
    public class RegisterResponse
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TenantName { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public string Token { get; set; }
    }
}
