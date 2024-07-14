using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Tests.Common.DataGeneration;

public static partial class Fakers
{
    public static Faker<Student> StudentsFaker { get; } = new Faker<Student>()
        .CustomInstantiator(faker => new Student
        {
            Email = faker.Person.Email,
            Handle = faker.Person.FirstName
        });
}