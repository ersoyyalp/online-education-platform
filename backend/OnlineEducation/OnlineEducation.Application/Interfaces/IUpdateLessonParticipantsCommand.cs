using OnlineEducation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.Interfaces
{
    public interface IUpdateLessonParticipantsCommand
    {
        Task<bool> UpdateAsync(
        int instructorId,
        int lessonId,
        UpdateLessonParticipantsRequestDto request);
    }
}
