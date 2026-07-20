using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class UpdateProfileHandler : IUpdateProfileHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAccountService _userAccountService;

        public UpdateProfileHandler(ICurrentUserService currentUserService, IUserAccountService userAccountService)
        {
            _currentUserService = currentUserService;
            _userAccountService = userAccountService;
        }

        public async Task<AppResponse<UpdateProfileResponse>> HandleAsync(UpdateProfileRequest request)
        {
            var result = new AppResponse<UpdateProfileResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existing = await _userAccountService.FindByEmailAsync(request.Email);
                    if (existing != null)
                        return result.BuildError("Email is already in use.");

                    user.Email = request.Email;
                    user.UserName = request.Email;
                }

                user.PhoneNumber = request.PhoneNumber;

                var (succeeded, errors) = await _userAccountService.UpdateAsync(user);
                if (!succeeded)
                    return result.BuildError("Cannot update profile: " + string.Join(", ", errors));

                var response = new UpdateProfileResponse
                {
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                };
                result.BuildResult(response, "Profile has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
