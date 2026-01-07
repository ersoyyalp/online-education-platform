using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;

public class CreateLessonForParticipantRequestCommand
    : ICreateLessonForParticipantRequestCommand
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateLessonForParticipantRequestCommand(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(
        int participantId,
        CreateLessonForParticipantRequestDto dto)
    {
        using var connection = _connectionFactory.CreateConnection();

        // 1️⃣ Participant → InstructorId
        const string instructorSql = @"
SELECT InstructorId
FROM Participants
WHERE ParticipantId = @ParticipantId
  AND IsDeleted = 0;
";

        var instructorId = await connection.ExecuteScalarAsync<int?>(
            instructorSql,
            new { ParticipantId = participantId }
        );

        if (instructorId == null)
            throw new Exception("Participant için instructor bulunamadı.");

        // 2️⃣ Lesson Request INSERT
        const string insertSql = @"
INSERT INTO LessonRequest
(ParticipantId, InstructorId, Title, Description, StartTime, EndTime, Status)
VALUES
(@ParticipantId, @InstructorId, @Title, @Description, @StartTime, @EndTime, 'Pending');
";

        await connection.ExecuteAsync(insertSql, new
        {
            ParticipantId = participantId,
            InstructorId = instructorId.Value,
            dto.Title,
            dto.Description,
            dto.StartTime,
            dto.EndTime
        });
    }
}
