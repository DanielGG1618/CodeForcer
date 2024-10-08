using CodeForcer.Features.Students.Common.Models;
using EmailType = CodeForcer.Features.Students.Common.Models.Email;

namespace CodeForcer.Tests.Features.Students.Common;

public sealed record StudentData(string? Email, string Handle)
{
    public Student ToDomain() =>
        Email is null ? throw new ArgumentNullException(nameof(Email))
            : new(
                EmailType.Create(Email),
                Handle
            );

    public static Faker<StudentData> Faker { get; } = new Faker<StudentData>()
        .CustomInstantiator(fake =>
            {
                var fakeHandle = fake.Person.FirstName;
                FakeHandleValidator.ValidHandles.Add(fakeHandle);
                return new(fake.Person.Email, fakeHandle);
            }
        );
}
