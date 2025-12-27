using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Infrastructure.Dapper.Queries
{
    public class GetRescheduleRequestsQuery : IGetRescheduleRequestsQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetRescheduleRequestsQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<RescheduleRequestDto>> GetPendingAsync(int instructorId)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
        SELECT
            rr.RescheduleRequestId,
            rr.LessonScheduleId,
            rr.RequestedBy,
            rr.NewStartTime,
            rr.NewEndTime,
            rr.Status
        FROM RescheduleRequests rr
        INNER JOIN LessonSchedules ls ON ls.LessonScheduleId = rr.LessonScheduleId
        INNER JOIN Lessons l ON l.LessonId = ls.LessonId
        WHERE
            l.InstructorId = @InstructorId
            AND rr.Status = 'Pending'
            AND rr.IsDeleted = 0
        ORDER BY rr.CreatedAt;
        ";

            return (await connection.QueryAsync<RescheduleRequestDto>(
                sql,
                new { InstructorId = instructorId }
            )).ToList();
        }
    }

}
