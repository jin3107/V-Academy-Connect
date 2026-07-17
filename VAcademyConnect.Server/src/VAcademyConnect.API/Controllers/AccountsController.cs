using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.DTOs.Authentication.Requests;

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

        public AccountsController(
            ILoginHandler loginHandler,
            IRegisterHandler registerHandler,
            ILogoutHandler logoutHandler,
            IRefreshTokenHandler refreshTokenHandler,
            IChangePasswordHandler changePasswordHandler,
            ISendOtpHandler sendOtpHandler,
            IVerifyOtpHandler verifyOtpHandler,
            ILoginByGoogleHandler loginByGoogleHandler,
            IGetRolesHandler getRolesHandler)
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
                // Lưu token vào httpOnly cookie
                SetAuthCookie(result.Data.Token);

                // KHÔNG trả token trong response body
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
                // Lưu token vào httpOnly cookie
                SetAuthCookie(result.Data.Token);

                // KHÔNG trả token trong response body
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

            // Xóa cookie
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
                // Lưu token vào httpOnly cookie
                SetAuthCookie(result.Data.Token);

                // KHÔNG trả token trong response body
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
    }
}
