using Dapper;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Infrastructure.Dapper.Queries
{
    public class ApproveRescheduleRequestCommand : IApproveRescheduleRequestCommand
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApproveRescheduleRequestCommand(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task ApproveAsync(int requestId, int instructorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var tx = connection.BeginTransaction();

            try
            {
                // 1️⃣ Talep bilgisi
                var request = await connection.QuerySingleOrDefaultAsync(
                    @"
                SELECT rr.RescheduleRequestId,
                       rr.LessonScheduleId,
                       rr.NewStartTime,
                       rr.NewEndTime
                FROM RescheduleRequests rr
                INNER JOIN LessonSchedules ls ON ls.LessonScheduleId = rr.LessonScheduleId
                INNER JOIN Lessons l ON l.LessonId = ls.LessonId
                WHERE rr.RescheduleRequestId = @Id
                  AND l.InstructorId = @InstructorId
                  AND rr.Status = 'Pending';
                ",
                    new { Id = requestId, InstructorId = instructorId },
                    tx
                );

                if (request == null)
                    throw new InvalidOperationException("Talep bulunamadı veya yetkiniz yok.");

                // 2️⃣ Schedule güncelle
                await connection.ExecuteAsync(
                    @"
                UPDATE LessonSchedules
                SET StartTime = @NewStart,
                    EndTime = @NewEnd,
                    UpdatedAt = SYSUTCDATETIME(),
                    UpdatedBy = 'instructor'
                WHERE LessonScheduleId = @ScheduleId;
                ",
                    new
                    {
                        ScheduleId = request.LessonScheduleId,
                        NewStart = request.NewStartTime,
                        NewEnd = request.NewEndTime
                    },
                    tx
                );

                // 3️⃣ Talebi Approved yap
                await connection.ExecuteAsync(
                    @"
                UPDATE RescheduleRequests
                SET Status = 'Approved'
                WHERE RescheduleRequestId = @Id;
                ",
                    new { Id = requestId },
                    tx
                );

                // 4️⃣ Diğer pending talepleri Reject
                await connection.ExecuteAsync(
                    @"
                UPDATE RescheduleRequests
                SET Status = 'Rejected'
                WHERE LessonScheduleId = @ScheduleId
                  AND Status = 'Pending'
                  AND RescheduleRequestId <> @Id;
                ",
                    new
                    {
                        ScheduleId = request.LessonScheduleId,
                        Id = requestId
                    },
                    tx
                );

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }

}
