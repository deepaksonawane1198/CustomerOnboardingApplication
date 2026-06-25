using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories;

public class SqlWorkflowAuditRepository : IWorkflowAuditRepository
{
    private readonly IConfiguration _configuration;

    public SqlWorkflowAuditRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection CreateConnection()
    {
        var connectionString = _configuration["SqlConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("SqlConnectionString is missing from configuration.");
        }

        return new SqlConnection(connectionString);
    }

    public async Task CreateAsync(WorkflowAuditRecord record, CancellationToken cancellationToken = default)
    {
        const string sql = @"
INSERT INTO dbo.CustomerOnboardingData
(
    ApplicationId,
    InstanceId,
    ApplicantType,
    CompanyName,
    TaxId,
    Country,
    ContactEmail,
    SourceSystem,
    WorkflowStatus,
    CurrentStage,
    FinalReason,
    DownstreamRecordId,
    RequestPayloadJson,
    SubmittedAtUtc,
    CreatedAtUtc,
    LastUpdatedAtUtc
)
VALUES
(
    @ApplicationId,
    @InstanceId,
    @ApplicantType,
    @CompanyName,
    @TaxId,
    @Country,
    @ContactEmail,
    @SourceSystem,
    @WorkflowStatus,
    @CurrentStage,
    @FinalReason,
    @DownstreamRecordId,
    @RequestPayloadJson,
    @SubmittedAtUtc,
    @CreatedAtUtc,
    @LastUpdatedAtUtc
);";

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@ApplicationId", record.ApplicationId);
        command.Parameters.AddWithValue("@InstanceId", record.InstanceId);
        command.Parameters.AddWithValue("@ApplicantType", (object?)record.ApplicantType ?? DBNull.Value);
        command.Parameters.AddWithValue("@CompanyName", (object?)record.CompanyName ?? DBNull.Value);
        command.Parameters.AddWithValue("@TaxId", (object?)record.TaxId ?? DBNull.Value);
        command.Parameters.AddWithValue("@Country", (object?)record.Country ?? DBNull.Value);
        command.Parameters.AddWithValue("@ContactEmail", (object?)record.ContactEmail ?? DBNull.Value);
        command.Parameters.AddWithValue("@SourceSystem", (object?)record.SourceSystem ?? DBNull.Value);
        command.Parameters.AddWithValue("@WorkflowStatus", record.WorkflowStatus);
        command.Parameters.AddWithValue("@CurrentStage", (object?)record.CurrentStage ?? DBNull.Value);
        command.Parameters.AddWithValue("@FinalReason", (object?)record.FinalReason ?? DBNull.Value);
        command.Parameters.AddWithValue("@DownstreamRecordId", (object?)record.DownstreamRecordId ?? DBNull.Value);
        command.Parameters.AddWithValue("@RequestPayloadJson", record.RequestPayloadJson);
        command.Parameters.AddWithValue("@SubmittedAtUtc", record.SubmittedAtUtc);
        command.Parameters.AddWithValue("@CreatedAtUtc", record.CreatedAtUtc);
        command.Parameters.AddWithValue("@LastUpdatedAtUtc", record.LastUpdatedAtUtc);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkflowAuditUpdate update, CancellationToken cancellationToken = default)
    {
        const string sql = @"
UPDATE dbo.CustomerOnboardingData
SET
    WorkflowStatus = @WorkflowStatus,
    CurrentStage = @CurrentStage,
    FinalReason = @FinalReason,
    DownstreamRecordId = @DownstreamRecordId,
    LastUpdatedAtUtc = @LastUpdatedAtUtc
WHERE InstanceId = @InstanceId;";

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@InstanceId", update.InstanceId);
        command.Parameters.AddWithValue("@WorkflowStatus", update.WorkflowStatus);
        command.Parameters.AddWithValue("@CurrentStage", update.CurrentStage);
        command.Parameters.AddWithValue("@FinalReason", (object?)update.FinalReason ?? DBNull.Value);
        command.Parameters.AddWithValue("@DownstreamRecordId", (object?)update.DownstreamRecordId ?? DBNull.Value);
        command.Parameters.AddWithValue("@LastUpdatedAtUtc", update.LastUpdatedAtUtc);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}