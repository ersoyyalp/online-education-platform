using System.Data;
using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;

namespace OnlineEducation.Infrastructure.Dapper.Queries;

public class InstructorScheduleQuery : IInstructorScheduleQuery
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InstructorScheduleQuery(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<InstructorWeeklyScheduleDto>> GetWeeklyScheduleAsync(
        int instructorId,
        DateTime weekStart,
        DateTime weekEnd)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
SELECT 
    l.LessonId,
    l.Title AS LessonTitle,
    ls.StartTime,
    ls.EndTime,
    COUNT(lp.ParticipantId) AS ParticipantCount
FROM Lessons l
INNER JOIN LessonSchedules ls ON ls.LessonId = l.LessonId
LEFT JOIN LessonParticipants lp ON lp.LessonId = l.LessonId
WHERE 
    l.InstructorId = @InstructorId
    AND l.IsDeleted = 0
    AND ls.IsDeleted = 0
    AND ls.StartTime >= @WeekStart
    AND ls.EndTime <= @WeekEnd
GROUP BY
    l.LessonId,
    l.Title,
    ls.StartTime,
    ls.EndTime
ORDER BY
    ls.StartTime;
";

        var result = await connection.QueryAsync<InstructorWeeklyScheduleDto>(
            sql,
            new
            {
                InstructorId = instructorId,
                WeekStart = weekStart,
                WeekEnd = weekEnd
            });

        return result.ToList();
    }
}
