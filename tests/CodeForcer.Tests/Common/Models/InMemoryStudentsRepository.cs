using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Tests.Common.Models;

public class InMemoryStudentsRepository : IStudentsRepository
{
    private static readonly Dictionary<string, Student> Students = [];
    
    public static void Clear() => Students.Clear();
    
    public Task Add(Student student)
    {
        if (student.Email is null)
            throw new ArgumentNullException(nameof(student.Email));
        
        Students.Add(student.Email, student);
        return Task.CompletedTask;
    }

    public Task<Student?> GetByEmail(string email) =>
        Task.FromResult(Students.GetValueOrDefault(email));

    public Task<Student?> GetByHandle(string emailOrHandle) =>
        Task.FromResult(Students.Values.SingleOrDefault(s => s.Handle == emailOrHandle));

    public Task<IEnumerable<Student>> GetAll() => 
        Task.FromResult<IEnumerable<Student>>(Students.Values);
}