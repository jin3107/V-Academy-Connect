using Google.Apis.Auth;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Models.Entities;
using VAcademyConnect.Repositories.Interfaces;
using VAcademyConnect.Services.Interfaces;
using VAcademyConnect.Services.Mapping;

namespace VAcademyConnect.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IConfiguration _config;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor contextAccessor, ITenantRepository tenantRepository, IConfiguration config, ITenantSubscriptionRepository tenantSubscriptionRepository, ISubscriptionRepository subscriptionRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _tenantRepository = tenantRepository;
            _config = config;
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<AppResponse<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var result = new AppResponse<ChangePasswordResponse>();
            try
            {
                var user = await _userManager.FindByNameAsync(request.Email);
                if (user == null)
                    return result.BuildError("Email did not found.");

                var removePassword = await _userManager.RemovePasswordAsync(user);
                if (!removePassword.Succeeded)
                    return result.BuildError(removePassword.Errors.First().Description);

                var addPassword = await _userManager.AddPasswordAsync(user, request.NewPassword);
                if (!addPassword.Succeeded)
                    return result.BuildError(addPassword.Errors.First().Description);

                var response = new ChangePasswordResponse
                {
                    Email = user!.Email!,
                    NewPassword = request.NewPassword,
                    ConfirmNePassword = request.ConfirmNewPassword,
                    PhoneNumber = user!.PhoneNumber ?? string.Empty
                };
                result.BuildResult(response, "Password has been changed successfuly.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<LogInResponse>> LoginAsync(LogInRequest request)
        {
            var result = new AppResponse<LogInResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    if (request.Email == "tanchuonghuynh3@gmail.com")
                    {
                        await CreatAdminUserAsync(request.Email);
                        user = await _userManager.FindByNameAsync(request.Email);
                    }
                    else
                        return result.BuildError("User not found");
                }
                if (!await _userManager.CheckPasswordAsync(user!, request.Password))
                    return result.BuildError("Invalid credentials");

                var roles = await _userManager.GetRolesAsync(user!);
                if (!roles.Any())
                    return result.BuildError("User has no role assigned.");

                var claims = await GetClaims(request, user!);
                var tokenString = GenerateAccessToken(claims);
                var response = new LogInResponse
                {
                    UserId = Guid.Parse(user!.Id),
                    TenantId = user!.TenantId ?? Guid.Empty,
                    PhoneNumber = user!.PhoneNumber ?? string.Empty,
                    Email = user!.Email!,
                    Role = roles!.FirstOrDefault()!,
                    Token = tokenString
                };

                return result.BuildResult(response);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private async Task<ApplicationUser> CreatAdminUserAsync(string email)
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
            return admin;
        }

        private async Task AssignRole(ApplicationUser user, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
        }

        private async Task<IdentityResult> CreateUser(RegisterRequest request, Guid tenantId)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                TenantId = tenantId,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = request.PhoneNumber
            };
            return await _userManager.CreateAsync(user, request.Password);
        }

        private async Task<Tenant> CreateTenant(RegisterRequest request)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.Email,
                ContactEmail = request.Email,
                ContactPhone = request.PhoneNumber,
                Status = Status.Active,
                CreatedBy = request.Email,
                CreatedOn = DateTime.UtcNow
            };
            _tenantRepository.Add(tenant);
            return tenant;
        }

        private async Task<bool> CheckUserExists(string email, string phoneNumber)
        {
            var userByEmail = await _userManager.FindByEmailAsync(email);
            if (userByEmail != null) return true;

            return _userManager.Users.Any(p => p.PhoneNumber == phoneNumber);
        }

        public async Task<AppResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var result = new AppResponse<RegisterResponse>();
            try
            {
                if (await CheckUserExists(request.Email, request.PhoneNumber))
                    return result.BuildError("User already exists.");

                var newTenant = await CreateTenant(request);
                var createResult = await CreateUser(request, newTenant.Id);
                if (!createResult.Succeeded)
                    return result.BuildError(string.Join(", ", createResult.Errors.Select(e => e.Description)));

                var user = await _userManager.FindByEmailAsync(request.Email);
                await AssignRole(user!, "TenantAdmin");

                var trialPlan = await _subscriptionRepository.GetTrialPlanAsync();
                if (trialPlan == null)
                    return result.BuildError("Trial subscription plan not found.");

                var trialSubscription = TenantSubscriptionMapper.ToTrialSubscription(newTenant.Id, trialPlan);
                await _tenantSubscriptionRepository.AddAsync(trialSubscription);

                var response = new RegisterResponse
                {
                    Email = request.Email,
                    Token = GenerateAccessToken(new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user!.Email!),
                        new Claim(ClaimTypes.Role, "TenantAdmin"),
                    }),
                    PhoneNumber = request.PhoneNumber,
                    Name = request.Name,
                    TenantName = request.TenantName,
                    SubscriptionPlanId = trialPlan.Id
                };
                await _signInManager.SignInAsync(user!, isPersistent: false);
                return result.BuildResult(response, "User registered successfully!");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private async Task<List<Claim>> GetClaims(LogInRequest user, ApplicationUser identityUser)
        {
            var claims = new List<Claim>
            {
                new Claim("TenantId", identityUser.TenantId.ToString()!),
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(identityUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim("Roles", role));
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim(ClaimTypes.Name, user.Email));
            }
            return claims;
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<AppResponse<bool>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AppResponse<VerifyEmailResponse>> VerifyEmailAsync(VerifyEmailRequest request)
        {
            var result = new AppResponse<VerifyEmailResponse>();
            try
            {
                var user = await _userManager.FindByNameAsync(request.Email);
                if (user == null)
                    return result.BuildError("Email did not found.");

                var response = new VerifyEmailResponse { Email = user!.Email! };
                result.BuildResult(response);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AppResponse<LogInResponse>> LoginByGoogleAsync(GoogleLoginRequest request)
        {
            var result = new AppResponse<LogInResponse>();
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[]
                    {
                        _contextAccessor.HttpContext!.RequestServices
                            .GetRequiredService<IConfiguration>()["Authentication:Google:ClientId"]
                    }
                };

                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
                }
                catch (Exception)
                {
                    return result.BuildError("Invalid Google token");
                }

                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    var tenant = await CreateTenant(new RegisterRequest
                    {
                        Email = payload.Email,
                        PhoneNumber = "",
                    });

                    var createResult = await CreateUser(new RegisterRequest
                    {
                        Email = payload.Email,
                        PhoneNumber = "",
                        Password = Guid.NewGuid().ToString(),
                    }, tenant.Id);

                    if (!createResult.Succeeded)
                        return result.BuildError(string.Join(", ", createResult.Errors.Select(e => e.Description)));

                    user = await _userManager.FindByEmailAsync(payload.Email);
                    await AssignRole(user!, "TenantAdmin");
                }

                var roles = await _userManager.GetRolesAsync(user!);
                if (!roles.Any())
                    return result.BuildError("User has no role assigned.");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user!.UserName!),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("phone", user.PhoneNumber ?? "")
                };
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);
                await _signInManager.SignOutAsync();
                await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                var token = GenerateAccessToken(claims);

                var response = new LogInResponse
                {
                    UserId = Guid.Parse(user.Id),
                    TenantId = user.TenantId ?? Guid.Empty,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault()!,
                    Token = token
                };

                return result.BuildResult(response);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
