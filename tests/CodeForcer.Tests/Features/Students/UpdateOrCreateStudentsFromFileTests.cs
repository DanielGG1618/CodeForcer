using System.Text;
using System.Net.Http.Headers;
using CodeForcer.Tests.Common.Extensions;
using CodeForcer.Tests.Features.Students.Common;

namespace CodeForcer.Tests.Features.Students;

public class UpdateOrCreateStudentsFromFileTests(IntegrationTestWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    private record Summary(int Updated, int Created, List<string> InvalidEmails, List<string> InvalidHandles);

    [Fact]
    public async Task ShouldCreateAllStudentsFromFileAndReturnSummary_WhenAllStudentsAreValidAndNew()
    {
        // Arrange
        var studentDatas = StudentData.Faker.GenerateBetween(5, 50);

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in studentDatas)
            fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(studentDatas.Count);
        summary?.Updated.Should().Be(0);
        summary?.InvalidEmails.Should().BeEmpty();
        summary?.InvalidHandles.Should().BeEmpty();

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(studentDatas.ToDomain());
    }

    [Fact]
    public async Task ShouldUpdatedAllStudentsFromFileAndReturnSummary_WhenAllStudentsAreValidAndAlreadyCreated()
    {
        // Arrange
        var previousStudentDatas = StudentData.Faker.GenerateBetween(5, 50);
        await StudentsRepository.AddRange(previousStudentDatas.ToDomain());

        var updatedStudentDatas = previousStudentDatas.Select(existing =>
            StudentData.Faker.Generate() with { Email = existing.Email }
        ).ToList();

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in updatedStudentDatas)
            fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(0);
        summary?.Updated.Should().Be(updatedStudentDatas.Count);
        summary?.InvalidEmails.Should().BeEmpty();
        summary?.InvalidHandles.Should().BeEmpty();

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(updatedStudentDatas.ToDomain());
        dbStudents.Should().NotContain(previousStudentDatas.ToDomain());
    }

    [Fact]
    public async Task ShouldUpdateExistingAndCreateNewStudentsFromFileAndReturnSummary_WhenAllStudentsAreValid()
    {
        // Arrange
        var previousStudentDatas = StudentData.Faker.GenerateBetween(3, 25);
        await StudentsRepository.AddRange(previousStudentDatas.ToDomain());

        var updatedStudentDatas = previousStudentDatas.Select(existing =>
            StudentData.Faker.Generate() with { Email = existing.Email }
        ).ToList();

        var newStudentDatas = StudentData.Faker.GenerateBetween(3, 25);

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in updatedStudentDatas.Concat(newStudentDatas))
            fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(newStudentDatas.Count);
        summary?.Updated.Should().Be(updatedStudentDatas.Count);
        summary?.InvalidEmails.Should().BeEmpty();
        summary?.InvalidHandles.Should().BeEmpty();

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(newStudentDatas.ToDomain());
        dbStudents.Should().NotContain(previousStudentDatas.ToDomain());
        dbStudents.Should().Contain(updatedStudentDatas.ToDomain());
    }

    [Fact]
    public async Task ShouldUpdateExistingAndCreateNewValidStudentsFromFileAndReturnSummary_WhenThereAreInvalidEmails()
    {
        // Arrange
        var previousStudentDatas = StudentData.Faker.GenerateBetween(3, 25);
        await StudentsRepository.AddRange(previousStudentDatas.ToDomain());

        var updatedStudentDatas = previousStudentDatas.Select(existing =>
            StudentData.Faker.Generate() with { Email = existing.Email }
        ).ToList();

        var newStudentDatas = StudentData.Faker.GenerateBetween(3, 25);

        var invalidEmailStudentDatas = StudentData.Faker.GenerateBetween(2, 10)
            .Select(student => student with { Email = Guid.NewGuid().ToString() })
            .ToList();

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in updatedStudentDatas
            .Concat(newStudentDatas)
            .Concat(invalidEmailStudentDatas)
            .Shuffle()
        ) fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(newStudentDatas.Count);
        summary?.Updated.Should().Be(updatedStudentDatas.Count);
        summary?.InvalidEmails.Should().BeEquivalentTo(invalidEmailStudentDatas.Select(s => s.Email));
        summary?.InvalidHandles.Should().BeEmpty();

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(newStudentDatas.ToDomain());
        dbStudents.Should().Contain(updatedStudentDatas.ToDomain());
        dbStudents.Should().NotContain(previousStudentDatas.ToDomain());
    }

    [Fact]
    public async Task
        ShouldUpdateExistingAndCreateNewValidStudentsFromFileAndReturnSummary_WhenThereAreInvalidHandles()
    {
        // Arrange
        var previousStudentDatas = StudentData.Faker.GenerateBetween(3, 25);
        await StudentsRepository.AddRange(previousStudentDatas.ToDomain());

        var updatedStudentDatas = previousStudentDatas.Select(existing =>
            StudentData.Faker.Generate() with { Email = existing.Email }
        ).ToList();

        var newStudentDatas = StudentData.Faker.GenerateBetween(3, 25);

        var invalidHandleStudentDatas = StudentData.Faker.GenerateBetween(2, 10)
            .Select(student => student with { Handle = Guid.NewGuid().ToString() })
            .ToList();

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in updatedStudentDatas
            .Concat(newStudentDatas)
            .Concat(invalidHandleStudentDatas)
            .Shuffle()
        ) fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(newStudentDatas.Count);
        summary?.Updated.Should().Be(updatedStudentDatas.Count);
        summary?.InvalidEmails.Should().BeEmpty();
        summary?.InvalidHandles.Should().BeEquivalentTo(invalidHandleStudentDatas.Select(s => s.Handle));

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(newStudentDatas.ToDomain());
        dbStudents.Should().Contain(updatedStudentDatas.ToDomain());
        dbStudents.Should().NotContain(previousStudentDatas.ToDomain());
    }

    [Fact]
    public async Task
        ShouldUpdateExistingAndCreateNewValidStudentsFromFileAndReturnSummary_WhenThereAreInvalidHandlesAndEmails()
    {
        // Arrange
        var previousStudentDatas = StudentData.Faker.GenerateBetween(3, 25);
        await StudentsRepository.AddRange(previousStudentDatas.ToDomain());

        var updatedStudentDatas = previousStudentDatas.Select(existing =>
            StudentData.Faker.Generate() with { Email = existing.Email }
        ).ToList();

        var newStudentDatas = StudentData.Faker.GenerateBetween(3, 25);

        var invalidHandleStudentDatas = StudentData.Faker.GenerateBetween(2, 10)
            .Select(student => student with { Handle = Guid.NewGuid().ToString() })
            .ToList();

        var invalidEmailStudentDatas = StudentData.Faker.GenerateBetween(2, 10)
            .Select(student => student with { Email = Guid.NewGuid().ToString() })
            .ToList();

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in updatedStudentDatas
            .Concat(newStudentDatas)
            .Concat(invalidHandleStudentDatas)
            .Concat(invalidEmailStudentDatas)
            .Shuffle()
        ) fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(newStudentDatas.Count);
        summary?.Updated.Should().Be(updatedStudentDatas.Count);
        summary?.InvalidEmails.Should().BeEquivalentTo(invalidEmailStudentDatas.Select(s => s.Email));
        summary?.InvalidHandles.Should().BeEquivalentTo(invalidHandleStudentDatas.Select(s => s.Handle));

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(newStudentDatas.ToDomain());
        dbStudents.Should().Contain(updatedStudentDatas.ToDomain());
        dbStudents.Should().NotContain(previousStudentDatas.ToDomain());
    }

    [Fact]
    public async Task ShouldCreateAllStudentsFromFileAndReturnSummary_WhenAllStudentsAreValidAndNewUseHandleAsKey()
    {
        // Arrange
        var studentDatas = StudentData.Faker.GenerateBetween(5, 50);

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in studentDatas)
            fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file?use-handle-as-key", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(studentDatas.Count);
        summary?.Updated.Should().Be(0);
        summary?.InvalidEmails.Should().BeEmpty();
        summary?.InvalidHandles.Should().BeEmpty();

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(studentDatas.ToDomain());
    }

    [Fact]
    public async Task
        ShouldUpdatedAllStudentsFromFileAndReturnSummary_WhenAllStudentsAreValidAndAlreadyCreatedUseHandleAsKey()
    {
        // Arrange
        var previousStudentDatas = StudentData.Faker.GenerateBetween(3, 25);
        await StudentsRepository.AddRange(previousStudentDatas.ToDomain());

        var updatedStudentDatas = previousStudentDatas.Select(existing =>
            StudentData.Faker.Generate() with { Handle = existing.Handle }
        ).ToList();

        var fileContents = new StringBuilder("email,handle\n");
        foreach (var studentData in updatedStudentDatas)
            fileContents.AppendLine($"{studentData.Email},{studentData.Handle}");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(fileContents.ToString())));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        content.Add(fileContent, "file", "students.csv");

        // Act
        var response = await Client.PostAsync("students/file?useHandleAsKey=true", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<Summary>();
        summary?.Created.Should().Be(0);
        summary?.Updated.Should().Be(updatedStudentDatas.Count);
        summary?.InvalidEmails.Should().BeEmpty();
        summary?.InvalidHandles.Should().BeEmpty();

        var dbStudents = await StudentsRepository.GetAll();
        dbStudents.Should().Contain(updatedStudentDatas.ToDomain());
        dbStudents.Should().NotContain(previousStudentDatas.ToDomain());
    }
}
