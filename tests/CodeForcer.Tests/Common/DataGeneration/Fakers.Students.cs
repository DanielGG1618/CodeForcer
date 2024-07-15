using CodeForcer.Features.Students.Common.Domain;

namespace CodeForcer.Tests.Common.DataGeneration;

public static partial class Fakers
{
    public static Faker<Student> StudentsFaker { get; } = new Faker<Student>()
        .CustomInstantiator(faker =>
            Student.Create(faker.Person.Email, faker.Person.FirstName)
        );
}