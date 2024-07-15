using CodeForcer.Features.Students.Common.Domain;
using CodeForcer.Features.Students.Common.Interfaces;

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

    public Task<Student?> GetByEmail(string email) =>
        Task.FromResult(_students.GetValueOrDefault(email));

    public Task<Student?> GetByHandle(string emailOrHandle) =>
        Task.FromResult(_students.Values.SingleOrDefault(s => s.Handle == emailOrHandle));

    public Task<IEnumerable<Student>> GetAll() => 
        Task.FromResult<IEnumerable<Student>>(_students.Values);

    public Task Clear()
    {
        _students.Clear();
        return Task.CompletedTask;
    }
}