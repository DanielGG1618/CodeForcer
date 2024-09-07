using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Features.Contests.Common.Models;

public record Submission(
    SubmissionId Id,
    Student Author,
    bool IsSuccessful,
    int PassedTestCount,
    double? Points,
    string ProgrammingLanguage,
    DateTimeOffset SubmissionTime
)
{
    public double? Points { get; set; } = Points;

    public async Task MapHandlesToEmails(Func<string, Task<Email?>> map) =>
        Author.Email ??= await map(Author.Handle);
}
