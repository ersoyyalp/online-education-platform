using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class ParticipantListItemDto
    {
        public int ParticipantId { get; set; }
        public string FullName { get; set; } = null!;
    }

}
