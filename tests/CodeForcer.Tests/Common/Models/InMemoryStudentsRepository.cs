using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Tests.Common.Models;

public class InMemoryStudentsRepository : IStudentsRepository
{
    private readonly Dictionary<string, Student> _students = [];

    public Task Add(Student student)
    {
        if (student.Email is null)
            throw new ArgumentNullException(nameof(student.Email));

        _students.Add(student.Email, student);
        return Task.CompletedTask;
    }

    public Task AddRange(IEnumerable<Student> students)
    {
        foreach (var student in students)
            Add(student);

        return Task.CompletedTask;
    }

    public Task<Student?> GetByEmail(Email email) =>
        Task.FromResult(_students.GetValueOrDefault(email));

    public Task<Student?> GetByHandle(string emailOrHandle) =>
        Task.FromResult(_students.Values.SingleOrDefault(s => s.Handle == emailOrHandle));

    public Task<List<Student>> GetAll() =>
        Task.FromResult(_students.Values.ToList());

    public Task UpdateByEmail(Email email, Student student) =>
        Task.FromResult(_students[email] = student);

    public Task UpdateByHandle(string handle, Student student)
    {
        var email = _students.Values.Single(s => s.Handle == handle).Email!;
        _students[email] = student;
        return Task.CompletedTask;
    }

    public Task<bool> DeleteByEmail(Email email) =>
        Task.FromResult(_students.Remove(email));

    public Task<bool> ExistsByEmail(Email email) =>
        Task.FromResult(_students.ContainsKey(email));

    public Task<bool> ExistsByHandle(string handle) =>
        Task.FromResult(_students.Values.Any(s => s.Handle == handle));

    public Task Clear()
    {
        _students.Clear();
        return Task.CompletedTask;
    }
}
