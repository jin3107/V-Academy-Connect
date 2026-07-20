using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;

namespace VAcademyConnect.DTOs.Authentication.Requests
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }

        public TenantType TenantType { get; set; } = TenantType.Personal;
    }
}
