using CodeForcer.Features.Students.Common.Domain;

namespace CodeForcer.Features.Students.Common;

public static class StudentsErrors
{
    public static Error NotFound =>
        Error.NotFound(
            code: $"{nameof(Student)}.{nameof(NotFound)}",
            description: "Student is not found"
        );

    public static Error InvalidEmail =>
        Error.Validation(
            code: $"{nameof(Student)}.{nameof(InvalidEmail)}",
            description: "Provided email is not valid"
        );
}