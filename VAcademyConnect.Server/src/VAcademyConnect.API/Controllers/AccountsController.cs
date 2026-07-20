using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;

namespace VAcademyConnect.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILoginHandler _loginHandler;
        private readonly IRegisterHandler _registerHandler;
        private readonly ILogoutHandler _logoutHandler;
        private readonly IRefreshTokenHandler _refreshTokenHandler;
        private readonly IChangePasswordHandler _changePasswordHandler;
        private readonly ISendOtpHandler _sendOtpHandler;
        private readonly IVerifyOtpHandler _verifyOtpHandler;
        private readonly ILoginByGoogleHandler _loginByGoogleHandler;
        private readonly IGetRolesHandler _getRolesHandler;
        private readonly IUpdateProfileHandler _updateProfileHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantRepository _tenantRepository;

        public AccountsController(
            ILoginHandler loginHandler,
            IRegisterHandler registerHandler,
            ILogoutHandler logoutHandler,
            IRefreshTokenHandler refreshTokenHandler,
            IChangePasswordHandler changePasswordHandler,
            ISendOtpHandler sendOtpHandler,
            IVerifyOtpHandler verifyOtpHandler,
            ILoginByGoogleHandler loginByGoogleHandler,
            IGetRolesHandler getRolesHandler,
            IUpdateProfileHandler updateProfileHandler,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager,
            ITenantRepository tenantRepository)
        {
            _loginHandler = loginHandler;
            _registerHandler = registerHandler;
            _logoutHandler = logoutHandler;
            _refreshTokenHandler = refreshTokenHandler;
            _changePasswordHandler = changePasswordHandler;
            _sendOtpHandler = sendOtpHandler;
            _verifyOtpHandler = verifyOtpHandler;
            _loginByGoogleHandler = loginByGoogleHandler;
            _getRolesHandler = getRolesHandler;
            _updateProfileHandler = updateProfileHandler;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _tenantRepository = tenantRepository;
        }

        private void SetAuthCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(24)
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);
        }

        private void DeleteAuthCookie()
        {
            Response.Cookies.Delete("AuthToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest logIn)
        {
            var result = await _loginHandler.HandleAsync(logIn);

            if (result.IsSuccess && result.Data != null)
            {
                SetAuthCookie(result.Data.Token);

                result.Data.Token = null!;
                result.Data.RefreshToken = null!;
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _refreshTokenHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest signUp)
        {
            var result = await _registerHandler.HandleAsync(signUp);

            if (result.IsSuccess && result.Data != null)
            {
                SetAuthCookie(result.Data.Token);

                result.Data.Token = null!;
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            var result = await _sendOtpHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _verifyOtpHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _changePasswordHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _logoutHandler.HandleAsync();

            DeleteAuthCookie();

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("google")]
        public async Task<IActionResult> LoginByGoogle([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
                return BadRequest("IdToken không được để trống.");

            var result = await _loginByGoogleHandler.HandleAsync(request);
            if (result == null || !result.IsSuccess)
                return BadRequest("Đăng nhập thất bại.");

            if (result.Data != null)
            {
                SetAuthCookie(result.Data.Token);

                result.Data.Token = null!;
                result.Data.RefreshToken = null!;
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _getRolesHandler.HandleAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var tenantType = TenantType.Personal;
            if (user.TenantId.HasValue)
            {
                var tenant = await _tenantRepository.FindByAsync(t => t.Id == user.TenantId).FirstOrDefaultAsync();
                if (tenant != null)
                    tenantType = tenant.Type;
            }

            var response = new LogInResponse
            {
                UserId = Guid.Parse(user.Id),
                TenantId = user.TenantId ?? Guid.Empty,
                TenantType = tenantType,
                UserName = user.Email!,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var result = await _updateProfileHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
