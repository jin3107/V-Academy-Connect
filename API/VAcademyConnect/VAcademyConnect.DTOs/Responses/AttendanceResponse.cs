using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class AttendanceResponse : BaseDto
    {
        public Guid? ClassId { get; set; }
        public string? StudentId { get; set; }
        public DateTime? Date { get; set; }
        public bool IsPresent { get; set; }
        public bool CountAsPaid { get; set; }
    }
}
