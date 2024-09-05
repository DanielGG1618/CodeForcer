using System.Text;
using System.Net.Http.Headers;
using CodeForcer.Tests.Features.Students.Common;

namespace CodeForcer.Tests.Features.Students;

public class UpdateOrCreateStudentsFromFileTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldUpdateOrCreateStudentsFromFile()
    {
        // Arrange
        var students = StudentData.Faker.GenerateBetween(5, 50);

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var student in students)
            fileContents.AppendLine($"{student.Email},{student.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file", content);

        // Assert
        // response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        //var dbStudents = await StudentsRepository.GetAll();
        //dbStudents.Should().Contain(students);
    }
}
