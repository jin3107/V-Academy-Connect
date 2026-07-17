using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class AttendanceMapper
    {
        public static Attendance ToEntity(AttendanceRequest request)
        {
            return new Attendance
            {
                ClassId = request.ClassId,
                StudentId = request.StudentId,
                Date = request.Date,
                IsPresent = request.IsPresent,
                CountAsPaid = request.CountAsPaid,
            };
        }

        public static AttendanceResponse ToResponse(Attendance entity)
        {
            return new AttendanceResponse
            {
                Id = entity.Id,
                ClassId = entity.ClassId,
                ClassName = entity.Class?.Name,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.FullName,
                Date = entity.Date,
                IsPresent = entity.IsPresent,
                CountAsPaid = entity.CountAsPaid,
            };
        }
    }
}
