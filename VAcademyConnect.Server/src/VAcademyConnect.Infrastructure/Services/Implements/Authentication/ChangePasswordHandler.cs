using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Identity;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class ChangePasswordHandler : IChangePasswordHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpCodeRepository _otpCodeRepository;

        public ChangePasswordHandler(UserManager<ApplicationUser> userManager, IOtpCodeRepository otpCodeRepository)
        {
            _userManager = userManager;
            _otpCodeRepository = otpCodeRepository;
        }

        public async Task<AppResponse<ChangePasswordResponse>> HandleAsync(ChangePasswordRequest request)
        {
            var result = new AppResponse<ChangePasswordResponse>();
            try
            {
                var email = request.Email.Trim().ToLowerInvariant();

                var verifiedOtp = await _otpCodeRepository.FindRecentlyVerifiedAsync(email, OtpPurpose.ChangePassword);
                if (verifiedOtp == null)
                    return result.BuildError("Please verify the OTP sent to your email before changing your password.");

                var user = await _userManager.FindByEmailAsync(email);
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
                    Email = user.Email!,
                    NewPassword = request.NewPassword,
                    ConfirmNePassword = request.ConfirmNewPassword,
                    PhoneNumber = user.PhoneNumber ?? string.Empty
                };
                result.BuildResult(response, "Password has been changed successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
