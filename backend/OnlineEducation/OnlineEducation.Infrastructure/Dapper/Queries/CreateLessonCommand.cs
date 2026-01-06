using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using System.Data;

namespace OnlineEducation.Infrastructure.Dapper.Commands
{
    public class CreateLessonCommand : ICreateLessonCommand
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CreateLessonCommand(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(
            int instructorId,
            CreateLessonRequestDto request)
        {
            request.StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Local);
            request.EndTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Local);

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1️⃣ ÇAKIŞMA KONTROLÜ
                var conflictExists = await connection.ExecuteScalarAsync<int>(
                    @"
                    SELECT COUNT(1)
                    FROM LessonSchedules ls
                    INNER JOIN Lessons l ON l.LessonId = ls.LessonId
                    WHERE l.InstructorId = @InstructorId
                      AND l.IsDeleted = 0
                      AND ls.IsDeleted = 0
                      AND (
                            @StartTime < ls.EndTime
                        AND @EndTime   > ls.StartTime
                      )
                    ",
                    new
                    {
                        InstructorId = instructorId,
                        request.StartTime,
                        request.EndTime
                    },
                    transaction
                );

                if (conflictExists > 0)
                {
                    throw new InvalidOperationException(
                        "Bu saat aralığında başka bir dersiniz bulunmaktadır.");
                }

                // 2️⃣ LESSON INSERT
                var lessonId = await connection.ExecuteScalarAsync<int>(
                    @"
                    INSERT INTO Lessons
                        (InstructorId, Title, Description, MeetingProvider, MeetingUrl,
                         IsActive, IsDeleted, CreatedAt, CreatedBy)
                    VALUES
                        (@InstructorId, @Title, @Description, @MeetingProvider, @MeetingUrl,
                         1, 0, GETUTCDATE(), 'system');

                    SELECT CAST(SCOPE_IDENTITY() AS int);
                    ",
                    new
                    {
                        InstructorId = instructorId,
                        request.Title,
                        request.Description,
                        request.MeetingProvider,
                        request.MeetingUrl
                    },
                    transaction
                );

                // 3️⃣ SCHEDULE INSERT
                await connection.ExecuteAsync(
                    @"
                    INSERT INTO LessonSchedules
                        (LessonId, StartTime, EndTime, Status,
                         IsDeleted, CreatedAt, CreatedBy)
                    VALUES
                        (@LessonId, @StartTime, @EndTime, 'Planned',
                         0, GETUTCDATE(), 'system');
                    ",
                    new
                    {
                        LessonId = lessonId,
                        request.StartTime,
                        request.EndTime
                    },
                    transaction
                );

                // 4️⃣ PARTICIPANTS
                foreach (var participantId in request.ParticipantIds)
                {
                    await connection.ExecuteAsync(
                        @"
                        INSERT INTO LessonParticipants
                            (LessonId, ParticipantId, CreatedAt)
                        VALUES
                            (@LessonId, @ParticipantId, GETUTCDATE());
                        ",
                        new
                        {
                            LessonId = lessonId,
                            ParticipantId = participantId
                        },
                        transaction
                    );
                }

                transaction.Commit();
                return lessonId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
