using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;

public class UpdateLessonParticipantsCommand : IUpdateLessonParticipantsCommand
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateLessonParticipantsCommand(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> UpdateAsync(
        int instructorId,
        int lessonId,
        UpdateLessonParticipantsRequestDto request)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // 1️⃣ DERS VAR MI + EĞİTMENE AİT Mİ?
            var exists = await connection.ExecuteScalarAsync<int>(
                @"
                SELECT COUNT(1)
                FROM Lessons
                WHERE LessonId = @LessonId
                  AND InstructorId = @InstructorId
                  AND IsDeleted = 0
                ",
                new { LessonId = lessonId, InstructorId = instructorId },
                transaction
            );

            if (exists == 0)
                return false;

            // 2️⃣ EKLE
            foreach (var participantId in request.AddParticipantIds.Distinct())
            {
                await connection.ExecuteAsync(
                    @"
                    IF NOT EXISTS (
                        SELECT 1
                        FROM LessonParticipants
                        WHERE LessonId = @LessonId
                          AND ParticipantId = @ParticipantId
                    )
                    BEGIN
                        INSERT INTO LessonParticipants
                            (LessonId, ParticipantId, CreatedAt)
                        VALUES
                            (@LessonId, @ParticipantId, SYSUTCDATETIME());
                    END
                    ",
                    new
                    {
                        LessonId = lessonId,
                        ParticipantId = participantId
                    },
                    transaction
                );
            }

            // 3️⃣ ÇIKAR
            foreach (var participantId in request.RemoveParticipantIds.Distinct())
            {
                await connection.ExecuteAsync(
                    @"
                    DELETE FROM LessonParticipants
                    WHERE LessonId = @LessonId
                      AND ParticipantId = @ParticipantId
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
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
