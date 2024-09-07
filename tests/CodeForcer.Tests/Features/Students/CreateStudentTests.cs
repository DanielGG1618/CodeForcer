using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Models;
using CodeForcer.Tests.Features.Students.Common;
using Microsoft.AspNetCore.Mvc;

namespace CodeForcer.Tests.Features.Students;

public class CreateStudentTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldAddStudent_WhenDataIsValid()
    {
        //Arrange
        var studentData = StudentData.Faker.Generate();
        var request = new { studentData.Email, studentData.Handle };

        //Act
        var response = await Client.PostAsJsonAsync("/students", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseStudent = await response.Content.ReadFromJsonAsync<StudentData>();
        responseStudent.Should().BeEquivalentTo(studentData);

        var dbStudent = await StudentsRepository.GetByEmail(Email.Create(studentData.Email!));
        dbStudent.Should().BeEquivalentTo(studentData.ToDomain());
    }

    [Fact]
    public async Task ShouldReturnBadRequestWithProblemDetails_WhenEmailIsInvalid()
    {
        //Arrange
        var student = StudentData.Faker.Generate();
        var request = new { Email = "invalid-email", student.Handle };

        //Act
        var response = await Client.PostAsJsonAsync("/students", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.InvalidEmail.Description);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWithProblemDetails_WhenHandleIsInvalid()
    {
        //Arrange
        var student = StudentData.Faker.Generate() with { Handle = "invalid-handle" };
        var request = new { student.Email, student.Handle };

        //Act
        var response = await Client.PostAsJsonAsync("/students", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.InvalidHandle.Description);
    }
}
