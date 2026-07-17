using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class SendOtpHandler : ISendOtpHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpCodeRepository _otpCodeRepository;
        private readonly IEmailService _emailService;

        public SendOtpHandler(UserManager<ApplicationUser> userManager, IOtpCodeRepository otpCodeRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _otpCodeRepository = otpCodeRepository;
            _emailService = emailService;
        }

        public async Task<AppResponse<SendOtpResponse>> HandleAsync(SendOtpRequest request)
        {
            var result = new AppResponse<SendOtpResponse>();
            try
            {
                var email = request.Email.Trim().ToLowerInvariant();
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return result.BuildError("Email did not found.");

                await _otpCodeRepository.InvalidatePreviousAsync(email, request.Purpose);

                var otp = GenerateOtp();
                await _otpCodeRepository.AddAsync(new OtpCode
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Code = otp,
                    Purpose = request.Purpose,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false,
                    AttemptCount = 0
                });

                var subject = request.Purpose == OtpPurpose.ChangePassword
                    ? "Your password change verification code"
                    : "Your email verification code";

                await _emailService.SendEmailAsync(email, subject,
                    $"Your OTP code is: {otp}\nValid for 5 minutes.");

                var response = new SendOtpResponse { Email = email, Message = "OTP has been sent to your email." };
                return result.BuildResult(response, "OTP sent successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private static string GenerateOtp()
        {
            var bytes = new byte[4];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return (Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000).ToString("D6");
        }
    }
}
