using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Application.Interfaces
{
    public interface IDeleteLessonCommand
    {
        Task<bool> DeleteAsync(int instructorId, int lessonId);
    }
}
