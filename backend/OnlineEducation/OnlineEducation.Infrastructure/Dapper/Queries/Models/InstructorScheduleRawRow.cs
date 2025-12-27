using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Infrastructure.Dapper.Queries.Models
{
    internal class InstructorScheduleRawRow
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int? ParticipantId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
