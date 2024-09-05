using System.ComponentModel.DataAnnotations;
using CodeForcer.Backend.Common.Extensions;

namespace CodeForcer.Backend.Features.Students.Common.Models;

public readonly record struct Email
{
    public string Value { get; }

    public static Email Create(string value) =>
        IsValid(value)
            ? new Email(value)
            : throw StudentsErrors.InvalidEmail.ToArgumentException();

    public static ErrorOr<Email> SafeCreate(string value) =>
        IsValid(value)
            ? new Email(value)
            : StudentsErrors.InvalidEmail;

    public static bool IsValid(string email) =>
        email.Contains(' ') is false
        && Validator.IsValid(email);

    public static bool IsValid(string value, out Email email)
    {
        if (IsValid(value))
        {
            email = new(value);
            return true;
        }

        email = default;
        return false;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    private static EmailAddressAttribute Validator { get; } = new();

    private Email(string value) => Value = value;
}
