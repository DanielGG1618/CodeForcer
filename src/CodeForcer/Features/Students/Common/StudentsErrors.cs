using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Features.Students.Common;

public static class StudentsErrors
{
    public static Error NotFound { get; } = Error.NotFound(
        code: $"{nameof(Student)}.{nameof(NotFound)}",
        description: "Student is not found"
    );

    public static Error InvalidEmail { get; } = Error.Validation(
        code: $"{nameof(Student)}.{nameof(InvalidEmail)}",
        description: "Provided email is not valid"
    );

    public static Error InvalidHandle { get; } = Error.Validation(
        code: $"{nameof(Student)}.{nameof(InvalidHandle)}",
        description: "Provided handle is not valid"
    );

    public static Error EmailsDoesNotMatch { get; } = Error.Validation(
        code: $"{nameof(Student)}.{nameof(EmailsDoesNotMatch)}",
        description: "Email in the request body does not match the email in the route"
    );

    public static Error HandlesDoesNotMatch { get; } = Error.Validation(
        code: $"{nameof(Student)}.{nameof(HandlesDoesNotMatch)}",
        description: "Handle in the request body does not match the handle in the route"
    );
}
