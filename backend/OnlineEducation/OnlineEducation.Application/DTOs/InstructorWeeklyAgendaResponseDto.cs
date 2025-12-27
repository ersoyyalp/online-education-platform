using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class InstructorWeeklyAgendaResponseDto
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }

        public IReadOnlyList<InstructorWeeklyScheduleDto> Items { get; set; }
            = Array.Empty<InstructorWeeklyScheduleDto>();
    }
}
