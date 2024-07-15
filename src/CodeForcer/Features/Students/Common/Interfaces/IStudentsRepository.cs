using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Features.Students.Common.Interfaces;

public interface IStudentsRepository
{
    Task Add(Student student);
    
    Task<Student?> GetByEmail(string email);
    Task<Student?> GetByHandle(string handle);
    Task<IEnumerable<Student>> GetAll();

    Task Clear();
}