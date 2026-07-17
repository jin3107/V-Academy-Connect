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

        // Chỉ áp dụng khi tài khoản Google đăng nhập lần đầu (tạo tenant mới).
        public TenantType TenantType { get; set; } = TenantType.Personal;
    }
}
