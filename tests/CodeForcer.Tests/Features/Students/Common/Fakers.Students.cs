using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Tests.Common.DataGeneration;

public static partial class Fakers
{
    public static Faker<Student> StudentsFaker { get; } = new Faker<Student>()
        .CustomInstantiator(faker =>
            new Student(Email.Create(faker.Person.Email), faker.Person.FirstName)
        );
}
