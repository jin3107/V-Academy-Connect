using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class AttendanceResponse
    {
        public Guid Id { get; set; }
        public Guid? ClassId { get; set; }
        public string? ClassName { get; set; }
        public string? StudentId { get; set; }
        public string? StudentName { get; set; }
        public DateTime? Date { get; set; }
        public bool IsPresent { get; set; }
        public bool CountAsPaid { get; set; }
    }
}
