using Dapper;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;

public class DeleteLessonCommand : IDeleteLessonCommand
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteLessonCommand(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> DeleteAsync(int instructorId, int lessonId)
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

            // 2️⃣ LESSON SOFT DELETE
            await connection.ExecuteAsync(
                @"
                UPDATE Lessons
                SET
                    IsDeleted = 1,
                    UpdatedAt = SYSUTCDATETIME(),
                    UpdatedBy = 'system'
                WHERE LessonId = @LessonId
                ",
                new { LessonId = lessonId },
                transaction
            );

            // 3️⃣ SCHEDULE SOFT DELETE
            await connection.ExecuteAsync(
                @"
                UPDATE LessonSchedules
                SET
                    IsDeleted = 1,
                    UpdatedAt = SYSUTCDATETIME(),
                    UpdatedBy = 'system'
                WHERE LessonId = @LessonId
                ",
                new { LessonId = lessonId },
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
