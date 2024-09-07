using CodeForcer.Tests.Features.Students.Common;

namespace CodeForcer.Tests.Features.Students;

public class GetAllStudentsTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoStudents()
    {
        //Arrange skipped

        //Act
        var response = await Client.GetAsync("/students");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseStudents = await response.Content.ReadFromJsonAsync<List<StudentData>>();
        responseStudents.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnStudents_WhenThereAreStudents()
    {
        //Arrange
        var studentDatas = StudentData.Faker.GenerateBetween(3, 52);
        await StudentsRepository.AddRange(studentDatas.ToDomain());

        //Act
        var response = await Client.GetAsync("/students");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseStudents = await response.Content.ReadFromJsonAsync<List<StudentData>>();
        responseStudents.Should().BeEquivalentTo(studentDatas);
    }
}
