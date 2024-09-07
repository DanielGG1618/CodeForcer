namespace CodeForcer.Infrastructure.CodeForces;

public enum CfContestType
{
    Cf,
    Ioi,
    Icpc,
}

public enum CfPhase
{
    Before,
    Coding,
    PendingSystemTest,
    SystemTest,
    Finished,
}

public enum CfProblemType
{
    Programming,
    Question,
}

public enum CfParticipantType
{
    Contestant,
    Practice,
    Virtual,
    Manager,
    OutOfCompetition,
}

public enum CfVerdict
{
    Failed,
    Ok,
    Partial,
    CompilationError,
    RuntimeError,
    WrongAnswer,
    PresentationError,
    TimeLimitExceeded,
    MemoryLimitExceeded,
    IdlenessLimitExceeded,
    SecurityViolated,
    Crashed,
    InputPreparationCrashed,
    Challenged,
    Skipped,
    Testing,
    Rejected,
}

public enum CfTestset
{
    Samples,
    Pretests,
    Tests,
    Challenges,
    Tests1,
    Tests2,
    Tests3,
    Tests4,
    Tests5,
    Tests6,
    Tests7,
    Tests8,
    Tests9,
    Tests10,
}
