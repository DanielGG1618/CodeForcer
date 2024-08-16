using CodeForcer.Contracts;
using CodeForcer.Features.Students.Common;
using Microsoft.AspNetCore.Mvc;

namespace CodeForcer.Tests.Features.Students;

public class UpdateOrCreateStudentTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldUpdateStudentByEmail_WhenStudentExistsAndValidData()
    {
        // Arrange
        var existingStudent = Fakers.StudentsFaker.Generate();

        await StudentsRepository.Add(existingStudent);

        var updatedStudent = Fakers.StudentsFaker.Clone()
            .RuleFor(s => s.Email, _ => existingStudent.Email)
            .Generate();

        var request = new UpdateOrCreateStudentRequest(updatedStudent.Email!, updatedStudent.Handle);

        //Act
        var response = await Client.PutAsJsonAsync($"students/{existingStudent.Email}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbStudent = await StudentsRepository.GetByEmail(existingStudent.Email!);
        dbStudent.Should().BeEquivalentTo(updatedStudent);
    }

    [Fact]
    public async Task ShouldCreateStudent_WhenStudentDoesNotExistAndValidData()
    {
        // Arrange
        var student = Fakers.StudentsFaker.Generate();

        var request = new UpdateOrCreateStudentRequest(student.Email!, student.Handle);

        //Act
        var response = await Client.PutAsJsonAsync($"students/{student.Email}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseStudent = await response.Content.ReadFromJsonAsync<StudentResponse>();
        responseStudent.Should().BeEquivalentTo(student);

        var dbStudent = await StudentsRepository.GetByEmail(student.Email!);
        dbStudent.Should().BeEquivalentTo(student);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenEmailsDoNotMatch()
    {
        // Arrange
        var student = Fakers.StudentsFaker.Generate();

        var request = new UpdateOrCreateStudentRequest(student.Email!, student.Handle);

        //Act
        var response = await Client.PutAsJsonAsync($"students/{student.Email}-invalid", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.EmailsDoesNotMatch.Description);

        var dbStudent = await StudentsRepository.GetByEmail(student.Email!);
        dbStudent.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        var student = Fakers.StudentsFaker.Clone().RuleFor(s => s.Email, () => "invalidEmail").Generate();

        var request = new UpdateOrCreateStudentRequest(student.Email!, student.Handle);

        //Act
        var response = await Client.PutAsJsonAsync($"students/{student.Email}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.InvalidEmail.Description);

        var dbStudent = await StudentsRepository.GetByEmail(student.Email!);
        dbStudent.Should().BeNull();
    }
}

public record UpdateOrCreateStudentRequest(string Email, string Handle);
