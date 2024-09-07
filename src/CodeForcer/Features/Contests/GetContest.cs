using AutoApiGen.Attributes;
using CodeForcer.Features.Contests.Common.Interfaces;
using CodeForcer.Features.Contests.Common.Models;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeForcer.Features.Contests;

[GetEndpoint("contests/{Id:int}")]
public sealed record GetContestQuery(int Id, [FromQuery] string Key, [FromQuery] string Secret)
    : IQuery<ErrorOr<GetContestResponse>>;

public sealed record GetContestResponse(Contest Contest, List<Student> Participants);

public sealed class GetContestHandler(
    IContestsProvider contestsProvider,
    IStudentsRepository studentsRepository
) : IQueryHandler<GetContestQuery, ErrorOr<GetContestResponse>>
{
    private readonly IContestsProvider _contestsProvider = contestsProvider;
    private readonly IStudentsRepository _studentsRepository = studentsRepository;

    public async ValueTask<ErrorOr<GetContestResponse>> Handle(
        GetContestQuery getContestQuery,
        CancellationToken cancellationToken
    )
    {
        var (id, key, secret) = getContestQuery;

        var contest = await _contestsProvider.GetContest(new ContestId(id), key, secret);
        await contest.MapHandledToEmails(async handle =>
            (await _studentsRepository.GetByHandle(handle))?.Email
        );

        return new GetContestResponse(contest, contest.GetParticipants().ToList());
    }
}
