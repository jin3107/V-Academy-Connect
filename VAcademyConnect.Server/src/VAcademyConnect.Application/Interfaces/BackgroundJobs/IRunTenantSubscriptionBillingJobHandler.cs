using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.BackgroundJobs
{
    public interface IRunTenantSubscriptionBillingJobHandler
    {
        Task<AppResponse<string>> HandleAsync();
    }
}
