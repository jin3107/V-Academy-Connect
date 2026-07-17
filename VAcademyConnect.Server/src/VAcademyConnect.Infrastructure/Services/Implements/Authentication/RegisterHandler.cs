using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class RegisterHandler : IRegisterHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserAccountService _userAccountService;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITokenService _tokenService;

        public RegisterHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IUserAccountService userAccountService, ITenantRepository tenantRepository,
            ITenantSubscriptionRepository tenantSubscriptionRepository, ISubscriptionRepository subscriptionRepository,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userAccountService = userAccountService;
            _tenantRepository = tenantRepository;
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _subscriptionRepository = subscriptionRepository;
            _tokenService = tokenService;
        }

        public async Task<AppResponse<RegisterResponse>> HandleAsync(RegisterRequest request)
        {
            var result = new AppResponse<RegisterResponse>();
            try
            {
                if (await _userAccountService.ExistsByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                    return result.BuildError("User already exists.");

                var newTenant = await CreateTenant(request);

                var newUser = new ApplicationUser
                {
                    FullName = request.Name,
                    UserName = request.Email,
                    Email = request.Email,
                    TenantId = newTenant.Id,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = request.PhoneNumber,
                    Role = Role.TenantAdmin
                };

                var (createSucceeded, createErrors, createdUser) = await _userAccountService.CreateAsync(newUser, request.Password);
                if (!createSucceeded)
                    return result.BuildError(string.Join(", ", createErrors));

                var user = await _userManager.FindByEmailAsync(request.Email);
                await _userAccountService.AssignRoleAsync(user!, "TenantAdmin");

                var standardPlan = await _subscriptionRepository.FindByAsync(s => s.Name == "Standard" && !s.IsDeleted)
                    .FirstOrDefaultAsync();

                if (standardPlan == null)
                    return result.BuildError("Standard subscription plan not found.");

                var trialSubscription = TenantSubscriptionMapper.ToTrialSubscription(newTenant.Id, standardPlan);
                trialSubscription.CreatedBy = request.Email;
                trialSubscription.CreatedOn = DateTime.UtcNow;

                await _tenantSubscriptionRepository.AddAsync(trialSubscription);

                var claims = await _tokenService.BuildClaimsAsync(user!);
                var (accessToken, _) = await _tokenService.GenerateTokensAsync(user!, claims);

                var response = new RegisterResponse
                {
                    Email = request.Email,
                    Token = accessToken,
                    PhoneNumber = request.PhoneNumber,
                    Name = request.Name,
                    TenantName = request.TenantName,
                    SubscriptionPlanId = standardPlan.Id
                };

                await _signInManager.SignInAsync(user!, isPersistent: false);
                return result.BuildResult(response, "User registered successfully with 30-day Standard trial!");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private async Task<Tenant> CreateTenant(RegisterRequest request)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.TenantName,
                ContactEmail = request.Email,
                ContactPhone = request.PhoneNumber,
                Status = Status.Active,
                Type = TenantType.Personal,
                CreatedBy = request.Email,
                CreatedOn = DateTime.UtcNow
            };
            await _tenantRepository.AddAsync(tenant);
            return tenant;
        }
    }
}
