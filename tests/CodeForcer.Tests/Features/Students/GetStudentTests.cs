using System.Net;
using CodeForcer.Tests.Common.DataGeneration;

namespace CodeForcer.Tests.Features.Students;

public class GetStudentTests(IntegrationTestWebAppFactory factory) 
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldReturn404NotFound_WhenStudentDoesNotExist()
    {
        //Arrange
        var student = Fakers.StudentsFaker.Generate();
        
        //Act
        var response = await Client.GetAsync($"/students/{student.Email}");
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}