using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Models.Entities;
using VAcademyConnect.Repositories.Interfaces;
using VAcademyConnect.Services.Interfaces;
using VAcademyConnect.Services.Mapping;

namespace VAcademyConnect.Services.Implements
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IHttpContextAccessor contextAccessor, ITenantRepository tenantRepository, UserManager<ApplicationUser> userManager)
        {
            _subscriptionRepository = subscriptionRepository;
            _contextAccessor = contextAccessor;
            _tenantRepository = tenantRepository;
            _userManager = userManager;
        }

        public async Task<AppResponse<SubscriptionResponse>> CreateAsync(SubscriptionRequest request)
        {
            var result = new AppResponse<SubscriptionResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var newSub = SubscriptionMapper.ToEntity(request);
                newSub.Id = Guid.NewGuid();
                newSub.Name = request.Name;
                newSub.Price = request.Price;
                newSub.BillingCycle = request.BillingCycle;
                newSub.TrialDays = request.TrialDays;
                newSub.CreatedOn = DateTime.UtcNow;
                newSub.CreatedBy = user.Email;

                await _subscriptionRepository.AddAsync(newSub);
                var response = SubscriptionMapper.ToResponse(newSub);
                result.BuildResult(response, "Subscription has been created successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<string>> DeleteAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize");

                var subscription = await _subscriptionRepository.GetAsync(id);
                if (subscription == null || subscription.IsDeleted == true)
                    return result.BuildError($"Subscription with ID {id} was not found or has been deleted.");
                subscription.IsDeleted = true;
                subscription.ModifiedBy = user.Email;

                await _subscriptionRepository.EditAsync(subscription);
                result.BuildResult($"Subscription with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<SubscriptionResponse>> GetByIdAsync(Guid id)
        {
            var result = new AppResponse<SubscriptionResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var subscription = await _subscriptionRepository.FindByAsync(sub => sub.Id == id).FirstOrDefaultAsync();
                if (subscription == null || subscription.IsDeleted == true)
                    return result.BuildError($"Subscription with ID {id} was not found or has been deleted.");

                var response = SubscriptionMapper.ToResponse(subscription);
                result.BuildResult(response, $"Subscription with ID {id} has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<SubscriptionResponse>> UpdateAsync(SubscriptionRequest request)
        {
            var result = new AppResponse<SubscriptionResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize");

                var subscription = await _subscriptionRepository.GetAsync(request.Id!.Value);
                if (subscription == null || subscription.IsDeleted == true)
                    return result.BuildError($"Subscription with ID {request.Id!.Value} was not found or has been deleted.");

                subscription.Name = request.Name;
                subscription.Price = request.Price;
                subscription.TrialDays = request.TrialDays;
                subscription.BillingCycle = request.BillingCycle;
                subscription.ModifiedBy = user.Email;
                subscription.ModifiedOn = DateTime.UtcNow;

                await _subscriptionRepository.EditAsync(subscription);
                var response = SubscriptionMapper.ToResponse(subscription);
                result.BuildResult(response, $"Subscription with ID {request.Id!.Value} has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
