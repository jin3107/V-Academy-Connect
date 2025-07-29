using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Models.Entities;

namespace VAcademyConnect.Services.Mapping
{
    public static class TenantMapper
    {
        public static TenantRequest ToRequest(Tenant entity)
        {
            return new TenantRequest
            {
                Name = entity.Name,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                Status = entity.Status
            };
        }

        public static Tenant ToEntity(TenantRequest request)
        {
            return new Tenant
            {
                Name = request.Name,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = request.Status
            };
        }

        public static TenantResponse ToResponse(Tenant entity)
        {
            return new TenantResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                Status = entity.Status
            };
        }
    }
}
