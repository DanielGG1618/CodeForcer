using CodeForcer.Features.Contests.Common.Interfaces;
using CodeForcer.Features.Contests.Common.Models;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Infrastructure.CodeForces;

public sealed class CodeForcesContestsProvider : IContestsProvider, IHandleValidator
{
    public async Task<Contest> GetContest(ContestId id, string apiKey, string apiSecret)
    {
        var cfSubmissions = await CodeForcesRequests.ContestStatus(id, apiKey, apiSecret);
        var (cfContest, cfProblems, _) = await CodeForcesRequests.ContestStandings(id, apiKey, apiSecret);

        var submissionsByProblemIndex = new Dictionary<string, List<Submission>>();

        foreach (var cfSubmission in cfSubmissions)
        {
            var problemIndex = cfSubmission.Problem.Index;
            var submission = new Submission(
                new SubmissionId(cfSubmission.Id),
                new Student(cfSubmission.Author.Members[0].Handle),
                cfSubmission.Verdict is CfVerdict.Ok,
                cfSubmission.PassedTestCount,
                cfSubmission.Points,
                cfSubmission.ProgrammingLanguage,
                DateTimeOffset.FromUnixTimeSeconds(cfSubmission.CreationTimeSeconds)
            );
            if (submissionsByProblemIndex.TryGetValue(problemIndex, out var list))
                list.Add(submission);
            else
                submissionsByProblemIndex.Add(problemIndex, [submission]);
        }

        var problems = cfProblems.Select(cfProblem =>
            new Problem(
                cfProblem.Index,
                cfProblem.Name,
                cfProblem.Points,
                submissionsByProblemIndex[cfProblem.Index]
            )
        ).ToList();

        foreach (var problem in problems)
        {
            if (problem.MaxPoints is null)
            {
                var maxPointsFromSubmissions = problem.Submissions.FirstOrDefault(s => s.IsSuccessful)?.Points
                    ?? throw new Exception("-52");

                if (maxPointsFromSubmissions is 0)
                    maxPointsFromSubmissions = 1;

                problem.MaxPoints = maxPointsFromSubmissions;
            }

            foreach (var submission in problem.Submissions)
            {
                submission.Points ??= submission.IsSuccessful ?
                    problem.MaxPoints : 0;
            }
        }

        return new Contest(
            id,
            cfContest.Name,
            DateTimeOffset.FromUnixTimeSeconds(cfContest.StartTimeSeconds ?? throw new Exception("69")),
            TimeSpan.FromSeconds(cfContest.DurationSeconds),
            problems
        );
    }

    public async Task<bool> IsValid(string handle) =>
        !handle.Contains(';')
        && await CodeForcesRequests.UserInfo(handle) is not null;
}
