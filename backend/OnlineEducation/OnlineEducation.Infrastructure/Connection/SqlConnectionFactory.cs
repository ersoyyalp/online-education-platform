using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineEducation.Application.Common;

namespace OnlineEducation.Infrastructure.Connection;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.DefaultConnection;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
