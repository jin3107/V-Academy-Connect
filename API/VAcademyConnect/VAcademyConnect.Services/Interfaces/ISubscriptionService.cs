using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<AppResponse<SubscriptionResponse>> CreateAsync(SubscriptionRequest request);
        Task<AppResponse<SubscriptionResponse>> GetByIdAsync(Guid id);
        Task<AppResponse<SubscriptionResponse>> UpdateAsync(SubscriptionRequest request);
        Task<AppResponse<string>> DeleteAsync(Guid id);
    }
}
