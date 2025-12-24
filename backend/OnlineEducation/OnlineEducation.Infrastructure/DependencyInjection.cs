using Microsoft.Extensions.DependencyInjection;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using OnlineEducation.Infrastructure.Dapper.Queries;

namespace OnlineEducation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IInstructorScheduleQuery, InstructorScheduleQuery>();

        return services;
    }
}
