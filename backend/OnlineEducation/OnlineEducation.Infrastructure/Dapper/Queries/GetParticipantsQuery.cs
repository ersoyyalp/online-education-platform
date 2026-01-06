using Dapper;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using OnlineEducation.Infrastructure.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Infrastructure.Dapper.Queries
{
    public class GetParticipantsQuery : IGetParticipantsQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetParticipantsQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<ParticipantListItemDto>> GetAllAsync(int instructorId)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
            SELECT
                p.ParticipantId,
                (p.FirstName + ' ' + p.LastName) AS FullName
            FROM Participants p
            JOIN Users u ON u.UserId = p.UserId
            WHERE p.InstructorId = @InstructorId
              AND p.IsDeleted = 0
              AND u.IsAdmin = 0;
            ";

            return (await connection.QueryAsync<ParticipantListItemDto>(
                sql,
                new { InstructorId = instructorId }
            )).ToList();
        }
    }
}
