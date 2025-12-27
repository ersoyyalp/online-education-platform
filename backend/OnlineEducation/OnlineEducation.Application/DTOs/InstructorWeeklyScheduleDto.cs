using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class InstructorWeeklyScheduleDto
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = null!;
        public DateOnly Date { get; set; }
        public string Day { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public int ParticipantCount { get; set; }

        public List<ScheduleParticipantDto> Participants { get; set; } = new();
    }
    public class ScheduleParticipantDto
    {
        public int ParticipantId { get; set; }
        public string FullName { get; set; } = null!;
    }
}
