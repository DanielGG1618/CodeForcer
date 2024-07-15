using CodeForcer.Contracts;

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
}