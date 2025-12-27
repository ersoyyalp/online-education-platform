using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class UserAuthDto
    {
        public int UserId { get; set; }

        public string Email { get; set; } = null!;

        /// <summary>
        /// Instructor | Participant | Admin
        /// </summary>
        public string Role { get; set; } = null!;

        // Domain bağları (opsiyonel)
        public int? InstructorId { get; set; }
        public int? ParticipantId { get; set; }
    }
}

