using Google.Apis.Auth;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class LoginByGoogleHandler : ILoginByGoogleHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserAccountService _userAccountService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITokenService _tokenService;

        public LoginByGoogleHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IUserAccountService userAccountService, ITenantRepository tenantRepository,
            IHttpContextAccessor contextAccessor, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userAccountService = userAccountService;
            _tenantRepository = tenantRepository;
            _contextAccessor = contextAccessor;
            _tokenService = tokenService;
        }

        public async Task<AppResponse<LogInResponse>> HandleAsync(GoogleLoginRequest request)
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
                    var tenant = await CreateTenant(payload.Email);

                    var newUser = new ApplicationUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        TenantId = tenant.Id,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        PhoneNumber = "",
                        Role = Role.TenantAdmin
                    };

                    var (createSucceeded, createErrors, _) = await _userAccountService.CreateAsync(newUser, Guid.NewGuid().ToString());
                    if (!createSucceeded)
                        return result.BuildError(string.Join(", ", createErrors));

                    user = await _userManager.FindByEmailAsync(payload.Email);
                    await _userAccountService.AssignRoleAsync(user!, "TenantAdmin");
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
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);
                await _signInManager.SignOutAsync();
                await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user!, claims);

                var response = new LogInResponse
                {
                    UserId = Guid.Parse(user.Id),
                    TenantId = user.TenantId ?? Guid.Empty,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault()!,
                    Token = accessToken,
                    RefreshToken = refreshToken
                };

                return result.BuildResult(response);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private async Task<Tenant> CreateTenant(string email)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                ContactEmail = email,
                ContactPhone = "",
                Status = Status.Active,
                Type = TenantType.Personal,
                CreatedBy = email,
                CreatedOn = DateTime.UtcNow
            };
            await _tenantRepository.AddAsync(tenant);
            return tenant;
        }
    }
}
