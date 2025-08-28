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

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest logIn)
        {
            var result = await _accountService.LoginAsync(logIn);
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
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return Ok();
        }

        [HttpPost("google")]
        public async Task<IActionResult> LoginByGoogle([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
                return BadRequest("IdToken không được để trống.");

            var result = await _accountService.LoginByGoogleAsync(request);
            if (result == null)
                return BadRequest("Đăng nhập thất bại.");

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
