using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.DTOs
{
    public class DecideRescheduleRequestDto
    {
        public bool Approve { get; set; }
        public string? RejectReason { get; set; }
    }

}
