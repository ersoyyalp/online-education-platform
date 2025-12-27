using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;

public class CreateRescheduleRequestCommand : ICreateRescheduleRequestCommand
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateRescheduleRequestCommand(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(
        int lessonScheduleId,
        CreateRescheduleRequestDto request)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        INSERT INTO RescheduleRequests
        (
            LessonScheduleId,
            RequestedBy,
            NewStartTime,
            NewEndTime,
            Status,
            CreatedAt,
            IsDeleted
        )
        VALUES
        (
            @LessonScheduleId,
            @RequestedBy,
            @NewStartTime,
            @NewEndTime,
            'Pending',
            SYSUTCDATETIME(),
            0
        );

        SELECT CAST(SCOPE_IDENTITY() AS int);
        ";

        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            LessonScheduleId = lessonScheduleId,
            RequestedBy = $"Participant:{request.ParticipantId}",
            NewStartTime = request.RequestedStartTime,
            NewEndTime = request.RequestedEndTime
        });
    }
}
