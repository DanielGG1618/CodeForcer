using System.ComponentModel.DataAnnotations;

namespace CodeForcer.Features.Students.Common.Domain;

public class Student
{
    public string? Email { get; private init; } 
    public string Handle { get; private init; }

    public static Student Create(string email, string handle) =>
        new EmailAddressAttribute().IsValid(email)
            ? new(email, handle)
            : throw new ArgumentException(StudentsErrors.InvalidEmail.Description);

    public static ErrorOr<Student> SafeCreate(string email, string handle) =>
        new EmailAddressAttribute().IsValid(email)
            ? new Student(email, handle)
            : StudentsErrors.InvalidEmail;
    
    private Student(string email, string handle) =>
        (Email, Handle) = (email, handle);
}