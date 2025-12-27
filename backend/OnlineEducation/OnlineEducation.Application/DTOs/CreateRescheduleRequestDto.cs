using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class CreateRescheduleRequestDto
    {
        public int ParticipantId { get; set; }
        public DateTime RequestedStartTime { get; set; }
        public DateTime RequestedEndTime { get; set; }
    }
}
