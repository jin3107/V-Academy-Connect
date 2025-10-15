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
    public static class ClassMapper
    {
        public static ClassRequest ToRequest(Class entity)
        {
            return new ClassRequest
            {
                Name = entity.Name,
                TeacherId = entity.TeacherId,
                TenantId = entity.TenantId,
                Tuition = entity.Tuition
            };
        }

        public static Class ToEntity(ClassRequest request)
        {
            return new Class
            {
                Name = request.Name,
                TeacherId = request.TeacherId,
                TenantId = request.TenantId,
                Tuition = request.Tuition
            };
        }

        public static ClassResponse ToResponse(Class entity)
        {
            return new ClassResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                TeacherId= entity.TeacherId,
                TenantId = entity.TenantId,
                Tuition = Decimal.Parse(entity.Tuition.ToString("F2")),
                TeacherName = entity.Teacher?.UserName,
                TenantName = entity.Tenant?.Name,
                CreatedBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
            };
        }
    }
}
