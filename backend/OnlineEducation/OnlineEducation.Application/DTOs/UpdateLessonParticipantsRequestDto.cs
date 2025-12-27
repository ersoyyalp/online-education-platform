using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class UpdateLessonParticipantsRequestDto
    {
        public List<int> AddParticipantIds { get; set; } = new();
        public List<int> RemoveParticipantIds { get; set; } = new();
    }
}
