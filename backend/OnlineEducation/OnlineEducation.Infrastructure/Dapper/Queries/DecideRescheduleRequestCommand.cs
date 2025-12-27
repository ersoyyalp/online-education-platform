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
    public class DecideRescheduleRequestCommand : IDecideRescheduleRequestCommand
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DecideRescheduleRequestCommand(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task DecideAsync(
            int instructorId,
            int requestId,
            DecideRescheduleRequestDto request)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (request.Approve)
                {
                    // 1️⃣ DERSİ GÜNCELLE
                    await connection.ExecuteAsync(
                        @"
                    UPDATE ls
                    SET
                        ls.StartTime = rr.RequestedStartTime,
                        ls.EndTime = rr.RequestedEndTime,
                        ls.UpdatedAt = SYSUTCDATETIME()
                    FROM LessonSchedules ls
                    INNER JOIN RescheduleRequests rr
                        ON rr.LessonId = ls.LessonId
                    INNER JOIN Lessons l
                        ON l.LessonId = ls.LessonId
                    WHERE rr.RescheduleRequestId = @RequestId
                      AND l.InstructorId = @InstructorId
                    ",
                        new { RequestId = requestId, InstructorId = instructorId },
                        transaction
                    );

                    // 2️⃣ TALEBİ ONAYLA
                    await connection.ExecuteAsync(
                        @"
                    UPDATE RescheduleRequests
                    SET
                        Status = 'Approved',
                        UpdatedAt = SYSUTCDATETIME()
                    WHERE RescheduleRequestId = @RequestId
                    ",
                        new { RequestId = requestId },
                        transaction
                    );
                }
                else
                {
                    await connection.ExecuteAsync(
                        @"
                    UPDATE RescheduleRequests
                    SET
                        Status = 'Rejected',
                        RejectReason = @RejectReason,
                        UpdatedAt = SYSUTCDATETIME()
                    WHERE RescheduleRequestId = @RequestId
                    ",
                        new
                        {
                            RequestId = requestId,
                            request.RejectReason
                        },
                        transaction
                    );
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

}
