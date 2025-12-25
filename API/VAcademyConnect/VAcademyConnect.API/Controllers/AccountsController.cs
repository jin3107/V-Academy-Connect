using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.Services.Interfaces;

namespace VAcademyConnect.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
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
            var result = await _accountService.LoginAsync(logIn);
            
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
            var result = await _accountService.RefreshTokenAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest signUp)
        {
            var result = await _accountService.RegisterAsync(signUp);
            
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
        [Route("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = await _accountService.VerifyEmailAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _accountService.ChangePasswordAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            
            // Xóa cookie
            DeleteAuthCookie();
            
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("google")]
        public async Task<IActionResult> LoginByGoogle([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
                return BadRequest("IdToken không được để trống.");

            var result = await _accountService.LoginByGoogleAsync(request);
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
            var result = await _accountService.GetAllRoles();
            return Ok(result);
        }
    }
}
