using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Mapping
{
    public static class RoleMapper
    {
        public static RoleResponse ToResponse(IdentityRole entity)
        {
            return new RoleResponse
            {
                Id = Guid.Parse(entity.Id),
                Name = entity.Name!
            };
        }
    }
}
