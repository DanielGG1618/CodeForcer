using CodeForcer.Contracts;
using CodeForcer.Features.Students.Common;
using Microsoft.AspNetCore.Mvc;

namespace CodeForcer.Tests.Features.Students;

public class CreateStudentTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldAddStudent_WhenDataIsValid()
    {
        //Arrange
        var student = Fakers.StudentsFaker.Generate();

        var request = new CreateStudentRequest(student.Email!, student.Handle);

        //Act
        var response = await Client.PostAsJsonAsync("/students", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseStudent = await response.Content.ReadFromJsonAsync<StudentResponse>();
        responseStudent.Should().BeEquivalentTo(student);

        var dbStudent = await StudentsRepository.GetByEmail(student.Email!);
        dbStudent.Should().BeEquivalentTo(student);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWithProblemDetails_WhenEmailIsInvalid()
    {
        //Arrange
        var student = Fakers.StudentsFaker.Generate();
        var request = new CreateStudentRequest("invalid-email", student.Handle);

        //Act
        var response = await Client.PostAsJsonAsync("/students", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be(StudentsErrors.InvalidEmail.Description);
    }

    //TODO add handle validation
}