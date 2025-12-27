using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;

namespace OnlineEducation.Infrastructure.Dapper.Queries
{
    public class AuthQuery : IAuthQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AuthQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<UserAuthDto?> ValidateUserAsync(
            string email,
            string password)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
            SELECT
                u.UserId,
                u.Email,
                u.Role,
                u.InstructorId,
                u.ParticipantId
            FROM Users u
            WHERE u.Email = @Email
              AND u.PasswordHash = HASHBYTES('SHA2_256', CONVERT(varchar(256), @Password))
              AND u.IsActive = 1
              AND u.IsDeleted = 0;
            ";

            return await connection.QuerySingleOrDefaultAsync<UserAuthDto>(
                sql,
                new
                {
                    Email = email,
                    Password = password
                });
        }
    }
}
