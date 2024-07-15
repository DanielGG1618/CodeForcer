using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Features.Students.Common.Extensions;

public static class StudentExtensions
{
    public static StudentResponse ToResponse(this Student student) =>
        new(student.Email, student.Handle);
}