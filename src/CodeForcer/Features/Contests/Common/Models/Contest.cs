using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Features.Contests.Common.Models;

public record Contest(
    ContestId Id,
    string Name,
    DateTimeOffset StartTime,
    TimeSpan Duration,
    List<Problem> Problems
)
{
    public async Task MapHandledToEmails(Func<string, Task<Email?>> map)
    {
        foreach (var problem in Problems)
            await problem.MapHandlesToEmails(map);
    }

    public ISet<Student> GetParticipants() => new HashSet<Student>(
        Problems.SelectMany(problem => problem.GetParticipants())
    );
}
