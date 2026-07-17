using System;

namespace VAcademyConnect.DTOs.Responses
{
    public class TeachingClassResponse
    {
        public Guid ClassId { get; set; }
        public string? ClassName { get; set; }
        public DayOfWeek? TeachingDayOfWeek { get; set; }
        public TimeSpan? TeachingStartTime { get; set; }
        public TimeSpan? TeachingEndTime { get; set; }
    }
}
