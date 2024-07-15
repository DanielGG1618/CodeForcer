using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Features.Students.Common;

public static class StudentsErrors
{
    public static Error NotFound =>
        Error.NotFound(
            code: $"{nameof(Student)}.{nameof(NotFound)}",
            description: "Student is not found"
        );
}