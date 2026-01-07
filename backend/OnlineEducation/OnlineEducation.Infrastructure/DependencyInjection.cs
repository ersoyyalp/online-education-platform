using Microsoft.Extensions.DependencyInjection;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using OnlineEducation.Infrastructure.Dapper.Commands;
using OnlineEducation.Infrastructure.Dapper.Queries;
using OnlineEducation.Infrastructure.Services;

namespace OnlineEducation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IInstructorScheduleQuery, InstructorScheduleQuery>();

        services.AddScoped<ICreateLessonCommand, CreateLessonCommand>();

        services.AddScoped<IUpdateLessonCommand, UpdateLessonCommand>();

        services.AddScoped<IDeleteLessonCommand, DeleteLessonCommand>();

        services.AddScoped<IUpdateLessonParticipantsCommand, UpdateLessonParticipantsCommand>();

        services.AddScoped<ICreateRescheduleRequestCommand, CreateRescheduleRequestCommand>();

        services.AddScoped<IGetRescheduleRequestsQuery, GetRescheduleRequestsQuery>();

        services.AddScoped<IApproveRescheduleRequestCommand, ApproveRescheduleRequestCommand>();

        services.AddScoped<IDecideRescheduleRequestCommand, DecideRescheduleRequestCommand>();

        services.AddScoped<IParticipantScheduleQuery, ParticipantScheduleQuery>();

        services.AddScoped<IGetParticipantsQuery, GetParticipantsQuery>();

        services.AddScoped<ICreateLessonForParticipantRequestCommand, CreateLessonForParticipantRequestCommand>();

        services.AddScoped<IAuthQuery, AuthQuery>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        return services;
    }
}
