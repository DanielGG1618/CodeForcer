using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Features.Contests.Common.Models;

public record Problem(
    string Index,
    string Name,
    double? MaxPoints,
    List<Submission> Submissions
)
{
    public double? MaxPoints { get; set; } = MaxPoints;

    public async Task MapHandlesToEmails(Func<string, Task<Email?>> map)
    {
        foreach (var submission in Submissions)
            await submission.MapHandlesToEmails(map);
    }

    public ISet<Student> GetParticipants() => new HashSet<Student>(
        Submissions.Select(submission => submission.Author)
    );
}
