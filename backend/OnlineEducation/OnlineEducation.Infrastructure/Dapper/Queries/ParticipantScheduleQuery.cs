using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using OnlineEducation.Infrastructure.Dapper.Queries.Models;
using System.Data;

namespace OnlineEducation.Infrastructure.Dapper.Queries
{
    internal class ParticipantScheduleQuery : IParticipantScheduleQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ParticipantScheduleQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<InstructorWeeklyAgendaResponseDto> GetWeeklyScheduleAsync(
            int participantId,
            int offset)
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday)
                                 .AddDays(offset * 7);
            var weekEnd = weekStart.AddDays(7);

            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
SELECT
    l.LessonId,
    l.Title AS LessonTitle,
    ls.StartTime,
    ls.EndTime,

    p.ParticipantId,
    p.FirstName,
    p.LastName
FROM LessonParticipants lp
INNER JOIN Lessons l ON l.LessonId = lp.LessonId
INNER JOIN LessonSchedules ls ON ls.LessonId = l.LessonId
LEFT JOIN LessonParticipants lp2 ON lp2.LessonId = l.LessonId
LEFT JOIN Participants p ON p.ParticipantId = lp2.ParticipantId

WHERE lp.ParticipantId = @ParticipantId
  AND l.IsDeleted = 0
  AND ls.IsDeleted = 0
  AND ls.StartTime >= @WeekStart
  AND ls.EndTime <= @WeekEnd

ORDER BY ls.StartTime;
";

            var rows = await connection.QueryAsync<InstructorScheduleRawRow>(
                sql,
                new
                {
                    ParticipantId = participantId,
                    WeekStart = weekStart,
                    WeekEnd = weekEnd
                }
            );

            var items = rows
                .GroupBy(r => new { r.LessonId, r.LessonTitle, r.StartTime, r.EndTime })
                .Select(g => new InstructorWeeklyScheduleDto
                {
                    LessonId = g.Key.LessonId,
                    LessonTitle = g.Key.LessonTitle,

                    Date = DateOnly.FromDateTime(g.Key.StartTime),
                    Day = g.Key.StartTime.ToString("dddd"),

                    StartTime = g.Key.StartTime.ToString("HH:mm"),
                    EndTime = g.Key.EndTime.ToString("HH:mm"),

                    Participants = g
                        .Where(x => x.ParticipantId.HasValue)
                        .Select(x => new ScheduleParticipantDto
                        {
                            ParticipantId = x.ParticipantId!.Value,
                            FullName = $"{x.FirstName} {x.LastName}"
                        })
                        .DistinctBy(x => x.ParticipantId)
                        .ToList(),

                    ParticipantCount = g.Count(x => x.ParticipantId.HasValue)
                })
                .ToList();

            return new InstructorWeeklyAgendaResponseDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                Items = items
            };
        }
    }
}
