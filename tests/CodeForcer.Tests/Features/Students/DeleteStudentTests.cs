using CodeForcer.Tests.Features.Students.Common;

namespace CodeForcer.Tests.Features.Students;

public class DeleteStudentTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldDelete_WhenStudentExists()
    {
        //Arrange
        var student = StudentData.Faker.Generate().ToDomain();
        await StudentsRepository.Add(student);

        //Act
        var response = await Client.DeleteAsync($"students/{student.Email}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var studentExists = await StudentsRepository.ExistsByEmail(student.Email!.Value);
        studentExists.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenStudentDoesNotExist()
    {
        //Arrange
        var student = StudentData.Faker.Generate();

        //Act 
        var response = await Client.DeleteAsync($"students/{student.Email}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
