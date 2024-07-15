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

    public static Error EmailsDoesNotMatch =>
        Error.Validation(
            code: $"{nameof(Student)}.{nameof(EmailsDoesNotMatch)}",
            description: "Email in the request body does not match the email in the route"
        );

    public static Error HandlesDoesNotMatch =>
        Error.Validation(
            code: $"{nameof(Student)}.{nameof(EmailsDoesNotMatch)}",
            description: "Handle in the request body does not match the handle in the route"
        );
}