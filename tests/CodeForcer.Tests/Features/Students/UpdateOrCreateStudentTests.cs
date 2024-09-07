using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Models;
using CodeForcer.Tests.Features.Students.Common;
using Microsoft.AspNetCore.Mvc;

namespace CodeForcer.Tests.Features.Students;

public class UpdateOrCreateStudentTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldUpdateStudentByEmail_WhenStudentExistsAndValidData()
    {
        // Arrange
        var existingStudentData = StudentData.Faker.Generate();
        var existingStudent = existingStudentData.ToDomain();
        await StudentsRepository.Add(existingStudent);

        var updatedStudent = StudentData.Faker.Generate() with { Email = existingStudentData.Email };

        var request = new { updatedStudent.Email, updatedStudent.Handle };

        //Act
        var response = await Client.PutAsJsonAsync($"students/{existingStudentData.Email}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbStudent = await StudentsRepository.GetByEmail(existingStudent.Email!.Value);
        dbStudent.Should().BeEquivalentTo(updatedStudent.ToDomain());
    }

    [Fact]
    public async Task ShouldCreateStudent_WhenStudentDoesNotExistAndValidData()
    {
        // Arrange
        var student = StudentData.Faker.Generate();

        var request = new { student.Email, student.Handle };

        //Act
        var response = await Client.PutAsJsonAsync($"students/{student.Email}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbStudent = await StudentsRepository.GetByEmail(Email.Create(student.Email!));
        dbStudent.Should().BeEquivalentTo(student.ToDomain());
    }

    [Fact]
    public async Task ShouldReturnBadRequestWithProblemDetails_WhenEmailsBothAreValidButDoNotMatch()
    {
        // Arrange
        var studentData = StudentData.Faker.Generate();

        var request = new { studentData.Email, studentData.Handle };

        //Act
        var response = await Client.PutAsJsonAsync($"students/some.other.{studentData.Email}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.EmailsDoesNotMatch.Description);

        var dbStudent = await StudentsRepository.GetByEmail(Email.Create(studentData.Email!));
        dbStudent.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWithProblemDetails_WhenHandlesBothAreValidButDoNotMatch()
    {
        // Arrange
        var studentData = StudentData.Faker.Generate();

        var request = new { studentData.Email, studentData.Handle };

        //Act
        var response = await Client.PutAsJsonAsync($"students/some-other-{studentData.Handle}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.HandlesDoesNotMatch.Description);

        var dbStudent = await StudentsRepository.GetByEmail(Email.Create(studentData.Email!));
        dbStudent.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWithProblemDetails_WhenEmailIsInvalid()
    {
        // Arrange
        var studentData = StudentData.Faker.Generate() with { Email = "invalidEmail" };

        var request = new { studentData.Email, studentData.Handle };

        //Act
        var response = await Client.PutAsJsonAsync($"students/{studentData.Email}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.InvalidEmail.Description);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWithProblemDetails_WhenHandleIsInvalid()
    {
        // Arrange
        var studentData = StudentData.Faker.Generate() with { Handle = "invalid-handle" };

        var request = new { studentData.Email, studentData.Handle };

        //Act
        var response = await Client.PutAsJsonAsync($"students/{studentData.Handle}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.InvalidHandle.Description);
    }
}
