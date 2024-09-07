namespace CodeForcer.Infrastructure.CodeForces;

public sealed record CfContest(
    int Id,
    string Name,
    CfContestType Type,
    CfPhase Phase,
    bool Frozen,
    int DurationSeconds,
    int? StartTimeSeconds = null,
    int? RelativeTimeSeconds = null,
    string? PreparedBy = null,
    string? WebsiteUrl = null,
    string? Description = null,
    int? Difficulty = null,
    string? Kind = null,
    string? IcpcRegion = null,
    string? Country = null,
    string? City = null,
    string? Season = null
);

public sealed record CfProblem(
    string Index,
    CfProblemType Type,
    string Name,
    List<string> Tags,
    int? ContestId = null,
    string? ProblemsetName = null,
    double? Points = null,
    int? Rating = null
);

public sealed record CfRankListRow(
    CfParty Party,
    int Rank,
    double Points,
    int Penalty,
    int SuccessfulHackCount,
    int UnsuccessfulHackCount,
    List<int> ProblemResults,
    int? LastSubmissionTimeSeconds = null
);

public sealed record CfSubmission(
    int Id,
    int CreationTimeSeconds,
    int RelativeTimeSeconds,
    CfProblem Problem,
    CfParty Author,
    string ProgrammingLanguage,
    CfVerdict? Verdict,
    CfTestset Testset,
    int PassedTestCount,
    int TimeConsumedMillis,
    int MemoryConsumedBytes,
    double? Points = null,
    int? ContestId = null
);

public sealed record CfParty(
    List<CfMember> Members,
    bool Ghost,
    CfParticipantType ParticipantType,
    int? TeamId = null,
    string? TeamName = null,
    int? Room = null,
    int? ContestId = null,
    int? StartTimeSeconds = null
);

public sealed record CfMember(
    string Handle,
    string? Name = null
);
