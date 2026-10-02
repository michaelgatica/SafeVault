using Microsoft.Data.Sqlite;
using SafeVaultApp.Models;

namespace SafeVaultApp.Data;

public class SafeVaultRepository
{
    private readonly string _connectionString;

    public SafeVaultRepository(IConfiguration configuration)
        : this(configuration.GetConnectionString("SafeVault") ?? "Data Source=SafeVault.db")
    {
    }

    public SafeVaultRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText =
            "CREATE TABLE IF NOT EXISTS VaultRecords (" +
            "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Title TEXT NOT NULL, " +
            "SecretValue TEXT NOT NULL, " +
            "Notes TEXT NOT NULL, " +
            "CreatedBy TEXT NOT NULL, " +
            "CreatedAt TEXT NOT NULL);";

        await command.ExecuteNonQueryAsync();
    }

    public async Task<VaultRecord> AddRecordAsync(VaultRecord record)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO VaultRecords (Title, SecretValue, Notes, CreatedBy, CreatedAt) " +
            "VALUES ($title, $secretValue, $notes, $createdBy, $createdAt); " +
            "SELECT last_insert_rowid();";

        command.Parameters.AddWithValue("$title", record.Title);
        command.Parameters.AddWithValue("$secretValue", record.SecretValue);
        command.Parameters.AddWithValue("$notes", record.Notes);
        command.Parameters.AddWithValue("$createdBy", record.CreatedBy);
        command.Parameters.AddWithValue("$createdAt", record.CreatedAt.ToString("O"));

        object? result = await command.ExecuteScalarAsync();
        record.Id = Convert.ToInt32(result);

        return record;
    }

    public async Task<IReadOnlyList<VaultRecord>> SearchRecordsAsync(string? searchText)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            command.CommandText =
                "SELECT Id, Title, SecretValue, Notes, CreatedBy, CreatedAt " +
                "FROM VaultRecords ORDER BY CreatedAt DESC;";
        }
        else
        {
            command.CommandText =
                "SELECT Id, Title, SecretValue, Notes, CreatedBy, CreatedAt " +
                "FROM VaultRecords " +
                "WHERE Title LIKE $searchText OR Notes LIKE $searchText " +
                "ORDER BY CreatedAt DESC;";

            command.Parameters.AddWithValue("$searchText", $"%{searchText.Trim()}%");
        }

        var records = new List<VaultRecord>();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            records.Add(new VaultRecord
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                SecretValue = reader.GetString(2),
                Notes = reader.GetString(3),
                CreatedBy = reader.GetString(4),
                CreatedAt = DateTime.Parse(reader.GetString(5))
            });
        }

        return records;
    }
}
