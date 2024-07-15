using System.Data.Common;
using System.Data.SQLite;
using CodeForcer.Features.Students.Common.Domain;
using CodeForcer.Features.Students.Common.Interfaces;

namespace CodeForcer.Infrastructure;

public sealed class StudentsRepository : IStudentsRepository, IDisposable, IAsyncDisposable
{
    private readonly SQLiteConnection _connection;

    public StudentsRepository(string connectionString)
    {
        _connection = new(connectionString);

        EnsureIsOpenAndCreated();
    }

    private void EnsureIsOpenAndCreated()
    {
        _connection.Open();

        using var command = _connection.CreateCommand();

        command.CommandText = """
                              CREATE TABLE IF NOT EXISTS students (
                                  email TEXT PRIMARY KEY,
                                  handle TEXT
                              );
                              """;
        command.ExecuteNonQuery();
    }

    public async Task Add(Student student) =>
        await ExecuteNonQuery("INSERT INTO students (email, handle) VALUES (@Email, @Handle);",
            new SQLiteParameter("@Email", student.Email),
            new SQLiteParameter("@Handle", student.Handle)
        );

    public async Task<Student?> GetByEmail(string email)
    {
        var reader = await ExecuteQuery("SELECT email, handle FROM students WHERE email = @Email;",
            new SQLiteParameter("@Email", email)
        );

        return await reader.ReadAsync() is true
            ? Student.Create(reader.GetString(0), reader.GetString(1))
            : null;
    }

    public async Task<Student?> GetByHandle(string handle)
    {
        var reader = await ExecuteQuery("SELECT email, handle FROM students WHERE handle = @Handle",
            new SQLiteParameter("@Handle", handle)
        );

        return await reader.ReadAsync() is true
            ? Student.Create(reader.GetString(0), reader.GetString(1))
            : null;
    }

    public async Task<IEnumerable<Student>> GetAll()
    {
        var reader = await ExecuteQuery("SELECT email, handle FROM students;");

        var students = new List<Student>();

        while (await reader.ReadAsync())
            students.Add(Student.Create(reader.GetString(0), reader.GetString(1)));

        return students;
    }

    public async Task UpdateByEmail(string email, Student student) =>
        await ExecuteNonQuery("UPDATE students SET handle = @Handle WHERE email = @Email;",
            new SQLiteParameter("@Email", student.Email),
            new SQLiteParameter("@Handle", student.Handle)
        );

    public async Task<bool> ExistsByEmail(string email)
    {
        var reader = await ExecuteQuery("SELECT email FROM students WHERE email = @Email;",
            new SQLiteParameter("@Email", email)
        );

        return await reader.ReadAsync();
    }

    public async Task Clear() =>
        await ExecuteNonQuery("DELETE FROM students;");

    private async Task ExecuteNonQuery(string commandText, params SQLiteParameter[] parameters)
    {
        await using var command = _connection.CreateCommand();

        command.CommandText = commandText;
        command.Parameters.AddRange(parameters);

        await command.ExecuteNonQueryAsync();
    }

    private async Task<DbDataReader> ExecuteQuery(string queryText, params SQLiteParameter[] parameters)
    {
        await using var command = _connection.CreateCommand();

        command.CommandText = queryText;
        command.Parameters.AddRange(parameters);

        return await command.ExecuteReaderAsync();
    }


    public void Dispose() =>
        _connection.Dispose();

    public async ValueTask DisposeAsync() =>
        await _connection.DisposeAsync();
}