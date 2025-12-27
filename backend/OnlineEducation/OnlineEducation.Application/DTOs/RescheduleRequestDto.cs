using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class RescheduleRequestDto
    {
        public int RescheduleRequestId { get; set; }
        public int LessonScheduleId { get; set; }
        public string RequestedBy { get; set; } = null!;
        public DateTime NewStartTime { get; set; }
        public DateTime NewEndTime { get; set; }
        public string Status { get; set; } = null!;
    }


}
