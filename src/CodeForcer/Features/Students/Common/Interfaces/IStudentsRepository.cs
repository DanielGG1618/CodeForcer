using CodeForcer.Features.Students.Common.Domain;

namespace CodeForcer.Features.Students.Common.Interfaces;

public interface IStudentsRepository
{
    Task Add(Student student);

    Task<Student?> GetByEmail(string email);
    Task<Student?> GetByHandle(string handle);
    Task<IEnumerable<Student>> GetAll();

    Task UpdateByEmail(string email, Student student);

    Task<bool> ExistsByEmail(string email);

    Task Clear();
}