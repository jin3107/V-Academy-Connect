using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Identity;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class VerifyOtpHandler : IVerifyOtpHandler
    {
        private const int MaxAttempts = 5;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpCodeRepository _otpCodeRepository;

        public VerifyOtpHandler(UserManager<ApplicationUser> userManager, IOtpCodeRepository otpCodeRepository)
        {
            _userManager = userManager;
            _otpCodeRepository = otpCodeRepository;
        }

        public async Task<AppResponse<bool>> HandleAsync(VerifyOtpRequest request)
        {
            var result = new AppResponse<bool>();
            try
            {
                var email = request.Email.Trim().ToLowerInvariant();
                var otpCode = await _otpCodeRepository.FindActiveAsync(email, request.Purpose);
                if (otpCode == null)
                    return result.BuildError("OTP invalid or expired.");

                if (otpCode.AttemptCount >= MaxAttempts)
                {
                    otpCode.IsUsed = true;
                    await _otpCodeRepository.UpdateAsync(otpCode);
                    return result.BuildError("Maximum verification attempts exceeded. Please request a new OTP.");
                }

                if (otpCode.Code != request.Code)
                {
                    otpCode.AttemptCount++;
                    await _otpCodeRepository.UpdateAsync(otpCode);
                    return result.BuildError($"Incorrect code. {MaxAttempts - otpCode.AttemptCount} attempt(s) remaining.");
                }

                otpCode.IsUsed = true;
                await _otpCodeRepository.UpdateAsync(otpCode);

                if (request.Purpose == OtpPurpose.VerifyEmail)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }
                }

                return result.BuildResult(true, "Verification successful.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
