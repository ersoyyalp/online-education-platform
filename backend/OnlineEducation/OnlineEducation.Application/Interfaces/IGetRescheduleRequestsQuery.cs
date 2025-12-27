using OnlineEducation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.Interfaces
{
    public interface IGetRescheduleRequestsQuery
    {
        //Task<IReadOnlyList<RescheduleRequestDto>> GetForInstructorAsync(int instructorId);

        Task<IReadOnlyList<RescheduleRequestDto>> GetPendingAsync(int instructorId);

    }

}
