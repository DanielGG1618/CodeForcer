using CodeForcer.Features.Contests.Common.Models;

namespace CodeForcer.Features.Contests.Common.Interfaces;

public interface IContestsProvider
{
    public Task<Contest> GetContest(ContestId id, string apiKey, string apiSecret);
}
