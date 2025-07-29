using MayNghien.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAcademyConnect.DTOs.Responses
{
    public class BackgroundJobResponse : BaseDto
    {
        public DateTime RunDate { get; set; }
        public Guid? TenantId { get; set; }
        public string? Result { get; set; }
        public string? LogDetail { get; set; }

        public string? TenantName { get; set; }
    }
}
