using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Services.Mapping
{
    public static class ClassStudentMapper
    {

        public static ClassStudentRequest ToRequest(ClassStudent entity)
        {
            return new ClassStudentRequest
            {
                ClassId = entity.ClassId,
                StudentId = entity.StudentId,
            };
        }

        public static ClassStudent ToEntity(ClassStudentRequest request)
        {
            return new ClassStudent
            {
                ClassId = request.ClassId,
                StudentId = request.StudentId,
            };
        }


        public static ClassStudentResponse ToResponse(ClassStudent entity)
        {
            return new ClassStudentResponse
            {
                Id = entity.Id,
                ClassId = entity.ClassId,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.FullName,
                Email = entity.Student?.Email,
                PhoneNumber = entity.Student?.PhoneNumber,
                CreatedBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
            };
        }
    }
}
