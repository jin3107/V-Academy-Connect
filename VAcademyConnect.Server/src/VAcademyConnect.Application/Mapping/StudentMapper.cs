using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class StudentMapper
    {
        public static StudentResponse ToResponse(ApplicationUser entity)
        {
            return new StudentResponse
            {
                Id = entity.Id,
                Name = entity.FullName ?? entity.Email ?? string.Empty,
                Email = entity.Email ?? string.Empty,
                PhoneNumber = entity.PhoneNumber ?? string.Empty,
                TenantId = entity.TenantId,
                TenantName = entity.Tenant?.Name,
            };
        }
    }
}
