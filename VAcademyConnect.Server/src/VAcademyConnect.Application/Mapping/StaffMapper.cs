using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class StaffMapper
    {
        public static StaffRequest ToRequest(Staff entity)
        {
            return new StaffRequest
            {
                Name = entity.Name,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                Address = entity.Address,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Password = entity.Password,
                Role = entity.Role,
            };
        }

        public static Staff ToEntity(StaffRequest request)
        {
            return new Staff
            {
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Address = request.Address,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = request.Password,
                Role = request.Role,
            };
        }

        public static StaffResponse ToResponse(Staff entity) 
        {
            return new StaffResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Role = entity.Role,
                Gender = entity.Gender,
                Address = entity.Address,
                DateOfBirth = entity.DateOfBirth,
                CreateBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
                ModifiedBy = entity.ModifiedBy,
                ModifiedOn = entity.ModifiedOn,
                Password = entity.Password
            };
        }
    }
}
