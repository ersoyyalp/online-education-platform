using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using System.Data;

public class UpdateLessonCommand : IUpdateLessonCommand
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateLessonCommand(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> UpdateAsync(
        int instructorId,
        int lessonId,
        UpdateLessonRequestDto request)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // 1️⃣ OWNERSHIP + VAR MI?
            var exists = await connection.ExecuteScalarAsync<int>(
                @"
                SELECT COUNT(1)
                FROM Lessons
                WHERE LessonId = @LessonId
                  AND InstructorId = @InstructorId
                  AND IsDeleted = 0
                ",
                new
                {
                    LessonId = lessonId,
                    InstructorId = instructorId
                },
                transaction
            );

            if (exists == 0)
                return false;

            // 2️⃣ ÇAKIŞMA KONTROLÜ (KENDİ DERSİ HARİÇ)
            var conflict = await connection.ExecuteScalarAsync<int>(
                @"
                SELECT COUNT(1)
                FROM LessonSchedules ls
                INNER JOIN Lessons l ON l.LessonId = ls.LessonId
                WHERE l.InstructorId = @InstructorId
                  AND l.IsDeleted = 0
                  AND ls.IsDeleted = 0
                  AND l.LessonId <> @LessonId
                  AND (
                        @StartTime < ls.EndTime
                    AND @EndTime   > ls.StartTime
                  )
                ",
                new
                {
                    InstructorId = instructorId,
                    LessonId = lessonId,
                    request.StartTime,
                    request.EndTime
                },
                transaction
            );

            if (conflict > 0)
                throw new InvalidOperationException(
                    "Bu saat aralığında başka bir dersiniz bulunmaktadır.");

            // 3️⃣ LESSON UPDATE
            await connection.ExecuteAsync(
                @"
                UPDATE Lessons
                SET
                    Title = @Title,
                    Description = @Description,
                    MeetingProvider = @MeetingProvider,
                    MeetingUrl = @MeetingUrl,
                    UpdatedAt = SYSUTCDATETIME(),
                    UpdatedBy = 'system'
                WHERE LessonId = @LessonId
                ",
                new
                {
                    LessonId = lessonId,
                    request.Title,
                    request.Description,
                    request.MeetingProvider,
                    request.MeetingUrl
                },
                transaction
            );

            // 4️⃣ SCHEDULE UPDATE
            await connection.ExecuteAsync(
                @"
                UPDATE LessonSchedules
                SET
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    Status = @Status,
                    UpdatedAt = SYSUTCDATETIME(),
                    UpdatedBy = 'system'
                WHERE LessonId = @LessonId
                  AND IsDeleted = 0
                ",
                new
                {
                    LessonId = lessonId,
                    request.StartTime,
                    request.EndTime,
                    request.Status
                },
                transaction
            );

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
