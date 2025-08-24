using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Models.Entities;
using VAcademyConnect.Repositories.Implements;
using VAcademyConnect.Repositories.Interfaces;
using VAcademyConnect.Services.Interfaces;
using VAcademyConnect.Services.Mapping;

namespace VAcademyConnect.Services.Implements
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public StaffService(IStaffRepository staffRepository, IHttpContextAccessor contextAccessor,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            IConfiguration config, IRefreshTokenRepository refreshTokenRepository)
        {
            _staffRepository = staffRepository;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AppResponse<StaffResponse>> CreateAsync(StaffRequest request)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                if (await CheckUserExists(request.Email, request.PhoneNumber))
                    return result.BuildError("Staff with this email or phone number already exists.");

                var createStaff = await CreateStaff(request);
                if (createStaff == null)
                    return result.BuildError("Failed to create staff.");

                var (createResult, createdUser) = await CreateUser(request, createStaff.Id);
                if (!createResult.Succeeded)
                {
                    await _staffRepository.DeleteAsync(createStaff);

                    if (createdUser != null)
                        await _userManager.DeleteAsync(createdUser);

                    return result.BuildError("Cannot create user: "
                        + string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                var identityUser = await _userManager.FindByEmailAsync(request.Email);
                if (identityUser == null)
                    return result.BuildError("Failed to retrieve created Staff.");

                await AssignRole(identityUser, request.Role);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, request.Role),
                };

                var (accessToken, refreshToken) = await GenerateTokens(identityUser, claims);

                var response = StaffMapper.ToResponse(createStaff);
                response.Token = accessToken;
                return result.BuildResult(response, "Staff created successfully.");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        private async Task<(string accessToken, string refreshToken)> GenerateTokens(ApplicationUser user, IEnumerable<Claim> claims)
        {
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenModel = new RefreshTokenModel
            {
                UserId = Guid.Parse(user.Id),
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = user.Email
            };

            await _refreshTokenRepository.AddAsync(refreshTokenModel);

            return (accessToken, refreshToken);
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

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<(IdentityResult Result, ApplicationUser? User)> CreateUser(StaffRequest request, Guid staffId)
        {
            var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Register request for User is invalid");

            var identityUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                TenantId = user!.TenantId,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = request.PhoneNumber,
                Role = Role.Staff
            };

            var result = await _userManager.CreateAsync(identityUser, request.Password);
            return (result, identityUser);
        }

        public async Task<AppResponse<string>> Delete(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var currentUser = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                var staff = await _staffRepository.GetAsync(id);
                if (staff == null)
                    return result.BuildError("Staff not found");

                staff.IsDeleted = true;
                staff.ModifiedBy = currentUser.Email;
                staff.ModifiedOn = DateTime.UtcNow;

                await _staffRepository.EditAsync(staff);
                return result.BuildResult("Successfully deleted", "Staff deleted successfully");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        public async Task<AppResponse<StaffResponse>> GetByIdAsync(Guid id)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                var staff = await _staffRepository.GetAsync(id);
                if (staff == null || staff.IsDeleted)
                    return result.BuildError("Staff not found");

                var response = StaffMapper.ToResponse(staff);
                return result.BuildResult(response);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        public async Task<AppResponse<StaffResponse>> UpdateAsync(StaffRequest request)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                if (request.Id == null)
                    return result.BuildError("Staff ID is required");

                var staff = await _staffRepository.GetAsync(request.Id.Value);
                if (staff == null || staff.IsDeleted)
                    return result.BuildError("Staff not found");

                if ((staff.Email != request.Email || staff.PhoneNumber != request.PhoneNumber) &&
                    await CheckUserExists(request.Email, request.PhoneNumber))
                    return result.BuildError("Email or phone number already in use");

                var currentEmail = staff.Email;
                staff.Name = request.Name;
                staff.Email = request.Email;
                staff.PhoneNumber = request.PhoneNumber;
                staff.DateOfBirth = request.DateOfBirth;
                staff.Age = request.Age;
                staff.Gender = request.Gender;
                staff.Address = request.Address;
                staff.Role = request.Role;
                staff.ModifiedBy = currentUser.Email;
                staff.ModifiedOn = DateTime.UtcNow;

                var identityUser = await _userManager.FindByEmailAsync(currentEmail);
                if (identityUser == null)
                    return result.BuildError("Associated user account not found");

                identityUser.UserName = request.Email;
                identityUser.Email = request.Email;
                identityUser.PhoneNumber = request.PhoneNumber;

                var updateResult = await _userManager.UpdateAsync(identityUser);
                if (!updateResult.Succeeded)
                    return result.BuildError("Failed to update user: " +
                        string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                await AssignRole(identityUser, request.Role);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, request.Role),
                };

                await _staffRepository.EditAsync(staff);
                var (accessToken, refreshToken) = await GenerateTokens(identityUser, claims);
                var response = StaffMapper.ToResponse(staff);
                response.Token = accessToken;

                return result.BuildResult(response, "Staff updated successfully");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        private async Task<bool> CheckUserExists(string email, string phoneNumber)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            var userByEmail = await _userManager.FindByEmailAsync(email);
            if (userByEmail != null) return true;

            var userByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return userByPhoneNumber != null;
        }

        private async Task<Staff> CreateStaff(StaffRequest request)
        {
            var currentUser = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request for Staff is invalid");

            var staff = StaffMapper.ToEntity(request);
            staff.Id = Guid.NewGuid();
            staff.CreatedBy = currentUser?.Email;
            staff.CreatedOn = DateTime.UtcNow;
            staff.TenantId = currentUser?.TenantId;
            staff.IsDeleted = false;

            await _staffRepository.AddAsync(staff);
            return staff;
        }

        private async Task AssignRole(ApplicationUser user, string role)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
        }

        public Task<SearchResponse<StaffResponse>> SearchAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
