using Microsoft.EntityFrameworkCore;
using Npgsql;
using Persistence.Database;

namespace Persistence.IntegrationTests;

public sealed class PostgresFixture : IAsyncLifetime
{
    private const string TestDatabaseName = "prawko_tests";

    private readonly string _connectionString = "Host=127.0.0.1;Port=5432;Password=123;Persist Security Info=True;Username=postgres;Database=prawko_tests";

    public async Task InitializeAsync()
    {
        await EnsureDatabaseExistsAsync();

        await using var context = CreateDbContext();
        await context.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
        await context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new AppDbContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        EnsureTestDatabaseName();

        await using var context = CreateDbContext();

        await context.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                "UserAnswers",
                "RefreshTokens",
                "ExamSessionQuestions",
                "ExamSessions",
                "QuestionCategories",
                "Answers",
                "Questions",
                "Users",
                "Roles",
                "Categories",
                "UserAiProgresses"
            RESTART IDENTITY CASCADE;
            """);
    }

    private async Task EnsureDatabaseExistsAsync()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_connectionString);
        var databaseName = connectionStringBuilder.Database ?? TestDatabaseName;

        connectionStringBuilder.Database = "postgres";

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM pg_database WHERE datname = @databaseName;";
        command.Parameters.AddWithValue("databaseName", databaseName);

        var exists = await command.ExecuteScalarAsync();

        if (exists is not null)
            return;

        await using var createCommand = connection.CreateCommand();
        createCommand.CommandText = $"""CREATE DATABASE "{databaseName.Replace("\"", "\"\"")}";""";
        await createCommand.ExecuteNonQueryAsync();
    }

    private void EnsureTestDatabaseName()
    {
        var databaseName = new NpgsqlConnectionStringBuilder(_connectionString).Database;

        if (databaseName?.Contains("test", StringComparison.OrdinalIgnoreCase) != true)
            throw new InvalidOperationException(
                $"Integration tests can only reset a test database. Current database name is '{databaseName}'.");
    }
}
