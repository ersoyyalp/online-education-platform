using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.Interfaces
{
    public interface IApproveRescheduleRequestCommand
    {
        Task ApproveAsync(int rescheduleRequestId, int instructorId);
    }

}
