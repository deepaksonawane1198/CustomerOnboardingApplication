using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Connectors;

public class SqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection CreateConnection()
    {
        var connectionString = _configuration["SqlConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("SqlConnectionString is missing from configuration.");
        }

        return new SqlConnection(connectionString);
    }
}