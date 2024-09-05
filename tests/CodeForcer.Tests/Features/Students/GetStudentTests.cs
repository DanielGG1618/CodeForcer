using CodeForcer.Tests.Features.Students.Common;

namespace CodeForcer.Tests.Features.Students;

public class GetStudentTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldReturn404NotFound_WhenGetByEmailStudentDoesNotExist()
    {
        //Arrange
        var student = StudentData.Faker.Generate();

        //Act
        var response = await Client.GetAsync($"/students/{student.Email}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturn404NotFound_WhenGetByHandleStudentDoesNotExist()
    {
        //Arrange
        var student = StudentData.Faker.Generate();

        //Act
        var response = await Client.GetAsync($"/students/{student.Handle}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnStudent_WhenGetByEmailStudentExists()
    {
        //Arrange
        var student = StudentData.Faker.Generate();
        await StudentsRepository.Add(student.ToDomain());

        //Act
        var response = await Client.GetAsync($"/students/{student.Email}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseStudent = await response.Content.ReadFromJsonAsync<StudentData>();
        responseStudent.Should().BeEquivalentTo(student);
    }

    [Fact]
    public async Task ShouldReturnStudent_WhenGetByHandleStudentExists()
    {
        //Arrange
        var student = StudentData.Faker.Generate();
        await StudentsRepository.Add(student.ToDomain());

        //Act
        var response = await Client.GetAsync($"/students/{student.Handle}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseStudent = await response.Content.ReadFromJsonAsync<StudentData>();
        responseStudent.Should().BeEquivalentTo(student);
    }
}
