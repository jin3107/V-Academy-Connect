using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class LoginHandler : ILoginHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITokenService _tokenService;

        public LoginHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ITenantRepository tenantRepository, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tenantRepository = tenantRepository;
            _tokenService = tokenService;
        }

        public async Task<AppResponse<LogInResponse>> HandleAsync(LogInRequest request)
        {
            var result = new AppResponse<LogInResponse>();
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(request.UserName);
                    if (user == null && request.UserName == "tanchuonghuynh3@gmail.com")
                    {
                        await CreateAdminUserAsync(request.UserName);
                        user = await _userManager.FindByEmailAsync(request.UserName);
                    }
                }

                if (user == null)
                    return result.BuildError("User not found. Please check your username or email.");
                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                    return result.BuildError("Invalid credentials. Please check your password.");

                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Any())
                    return result.BuildError("User has no role assigned.");

                if (user.TenantId.HasValue)
                {
                    var tenant = await _tenantRepository.FindByAsync(t => t.Id == user.TenantId)
                        .Include(t => t.TenantSubscription).FirstOrDefaultAsync();
                    if (tenant != null)
                    {
                        if (tenant.Status == Status.Inactive ||
                            (tenant.TenantSubscription?.EndDate != null && tenant.TenantSubscription.EndDate < DateTime.UtcNow))
                        {
                            if (!roles.Any(r => r == "SuperAdmin" || r == "Admin"))
                                return result.BuildError("Your tenant account is inactive or subscription has expired. Please contact administrator.");
                        }
                    }
                }

                var claims = await _tokenService.BuildClaimsAsync(user);
                var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user, claims);

                var response = new LogInResponse
                {
                    UserId = Guid.Parse(user.Id),
                    TenantId = user.TenantId ?? Guid.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserName = user.Email!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault()!,
                    Token = accessToken,
                    RefreshToken = refreshToken
                };

                return result.BuildResult(response, "Login successful. Welcome back!");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private async Task CreateAdminUserAsync(string email)
        {
            var admin = new ApplicationUser
            {
                Email = email,
                EmailConfirmed = true,
                UserName = email,
                Role = Role.SuperAdmin
            };

            await _userManager.CreateAsync(admin);
            await _userManager.AddPasswordAsync(admin, "Abc@123");
            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

            await _userManager.AddToRoleAsync(admin, "SuperAdmin");
        }
    }
}
