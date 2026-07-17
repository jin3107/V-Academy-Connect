using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.Tenants
{
    public class UpdateTenantHandler : IUpdateTenantHandler
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTenantHandler(ITenantRepository tenantRepository, ICurrentUserService currentUserService)
        {
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<TenantResponse>> HandleAsync(TenantRequest request)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var tenant = await _tenantRepository.FindByAsync(t => t.Id == request.Id)
                    .Include(ts => ts.TenantSubscription).ThenInclude(t => t!.Subscription).FirstOrDefaultAsync();

                if (tenant == null || tenant.IsDeleted == true)
                    return result.BuildError("Tenant was not found or has been deleted.");

                if (!await ValidateStatusChange(tenant, request.Status))
                {
                    return result.BuildError("You do not have permission to change tenant status from Inactive to Active. Only administrators can perform this action.");
                }

                if (tenant.Status == Status.Inactive && request.Status == Status.Active)
                {
                    if (tenant.TenantSubscription == null || tenant.TenantSubscription.EndDate <= DateTime.UtcNow)
                    {
                        return result.BuildError("Cannot activate tenant: No active subscription found or subscription has expired. Please renew the subscription first.");
                    }
                }

                tenant.Name = request.Name;
                tenant.ContactEmail = request.ContactEmail;
                tenant.ContactPhone = request.ContactPhone;
                tenant.Status = request.Status;
                tenant.ModifiedBy = user.Email;
                tenant.ModifiedOn = DateTime.UtcNow;

                await _tenantRepository.EditAsync(tenant);
                var updatedResponse = TenantMapper.ToResponse(tenant);

                string message = $"Tenant has been updated successfully.";
                if (tenant.Status == Status.Active && tenant.TenantSubscription?.EndDate != null)
                {
                    message += $" Subscription is valid until {tenant.TenantSubscription.EndDate.Value:yyyy-MM-dd}.";
                }

                result.BuildResult(updatedResponse, message);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private async Task<bool> ValidateStatusChange(Tenant tenant, Status newStatus)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
                return false;

            if (tenant.Status == Status.Inactive && newStatus == Status.Active)
            {
                var roles = await _currentUserService.GetRolesAsync(user);
                return roles.Any(r => r == "SuperAdmin" || r == "Admin");
            }

            return true;
        }
    }
}
