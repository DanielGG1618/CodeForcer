using CodeForcer.Backend.Features.Students.Common.Models;

namespace CodeForcer.Backend.Features.Students.Common.Interfaces;

public interface IStudentsRepository
{
    Task Add(Student student);

    Task<Student?> GetByEmail(Email email);
    Task<Student?> GetByHandle(string handle);
    Task<List<Student>> GetAll();

    Task UpdateByEmail(Email email, Student student);
    Task UpdateByHandle(string handle, Student student);

    Task<bool> DeleteByEmail(Email email);

    Task<bool> ExistsByEmail(Email email);

    Task<bool> ExistsByHandle(string handle);
}
