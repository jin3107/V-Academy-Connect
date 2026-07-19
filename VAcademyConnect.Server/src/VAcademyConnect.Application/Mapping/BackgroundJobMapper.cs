using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class BackgroundJobMapper
    {
        public static BackgroundJobResponse ToResponse(BackgroundJob entity)
        {
            return new BackgroundJobResponse
            {
                Id = entity.Id,
                RunDate = entity.RunDate,
                TenantId = entity.TenantId,
                TenantName = entity.Tenant?.Name,
                Result = entity.Result,
                LogDetail = entity.LogDetail,
            };
        }
    }
}
