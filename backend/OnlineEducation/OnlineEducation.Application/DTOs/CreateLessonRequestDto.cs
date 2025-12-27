using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class CreateLessonRequestDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public string? MeetingProvider { get; set; }
        public string? MeetingUrl { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<int> ParticipantIds { get; set; } = new();
    }

}
