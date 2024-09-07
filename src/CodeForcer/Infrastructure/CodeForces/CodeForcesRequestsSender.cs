using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CodeForcer.Infrastructure.CodeForces;

public static class CodeForcesRequests
{
    private const string ApiUrl = "https://codeforces.com/api/";

    public static async Task<(CfContest, CfProblem[], CfRankListRow[])> ContestStandings(
        int contestId,
        string key,
        string secret
    )
    {
        var response = await SendRequest("contest.standings",
            key,
            secret,
            new() { ["contestId"] = contestId, ["asManager"] = true }
        );

        if (response is null)
            throw new Exception("Aboba");

        var contest = GetContestFromData(response.Value.GetProperty("contest"));
        var problems = response.Value.GetProperty("problems").EnumerateArray().Select(GetProblemFromData).ToArray();
        var rows = response.Value.GetProperty("rows").EnumerateArray().Select(GetRankListRowFromData).ToArray();

        return (contest, problems, rows);
    }

    public static async Task<CfSubmission[]> ContestStatus(int contestId, string key, string secret)
    {
        var response = await SendRequest("contest.status",
            key,
            secret,
            new() { ["contestId"] = contestId, ["asManager"] = true }
        );

        if (response is null)
            throw new Exception("abobus");

        return response.Value.EnumerateArray().Select(GetSubmissionFromData).ToArray();
    }

    public static Task<JsonElement?> UserInfo(string handle) =>
        SendAnonymousRequest("user.info",
            new() { ["handle"] = handle, ["checkHistoricHandles"] = false }
        );

    private static async Task<JsonElement?> SendRequest(
        string methodName,
        string key,
        string secret,
        Dictionary<string, object?> parameters
    )
    {
        var rand = Random.Shared.Next(100_000, 999_999);

        parameters["time"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        parameters["apiKey"] = key;

        var paramsStr = string.Join("&", parameters.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}"));

        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes($"{rand}/{methodName}?{paramsStr}#{secret}"));
        var apiSig = $"{rand}{BitConverter.ToString(hashBytes).Replace("-", "").ToLower()}";

        using var client = new HttpClient();
        var response = await client.GetAsync($"{ApiUrl}{methodName}?{paramsStr}&apiSig={apiSig}");

        return await ProcessResponse(response);
    }

    private static async Task<JsonElement?> SendAnonymousRequest(
        string methodName,
        Dictionary<string, object?> parameters
    )
    {
        using var client = new HttpClient();

        return await ProcessResponse(await client.GetAsync(
                $"{ApiUrl}{methodName}?{
                    string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"))
                }"
            )
        );
    }

    private record Content(JsonElement? Status, JsonElement? Result, string? Comment);

    private static async Task<JsonElement?> ProcessResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadFromJsonAsync<Content>();

        if (content is null)
            throw new Exception("CodeForces is unavailable"); //503

        if (content.Status is null)
            throw new Exception("CodeForces is unavailable"); //503

        if (content.Status?.ValueEquals("FAILED") is true)
            throw new Exception($"CodeForces API: {content.Comment}"); //response.StatusCode

        return content.Result;
    }

    private static CfContest GetContestFromData(JsonElement contestData) => new(
        contestData.GetProperty("id").GetInt32(),
        contestData.GetProperty("name").GetString()!,
        Enum.Parse<CfContestType>(contestData.GetProperty("type").GetString()!.Replace("_", ""), ignoreCase: true),
        Enum.Parse<CfPhase>(contestData.GetProperty("phase").GetString()!.Replace("_", ""), ignoreCase: true),
        contestData.GetProperty("frozen").GetBoolean(),
        contestData.GetProperty("durationSeconds").GetInt32(),
        contestData.TryGetProperty("startTimeSeconds", out var property) ? property.GetInt32() : null,
        contestData.TryGetProperty("relativeTimeSeconds", out property) ? property.GetInt32() : null,
        contestData.TryGetProperty("preparedBy", out property) ? property.GetString() : null,
        contestData.TryGetProperty("websiteUrl", out property) ? property.GetString() : null,
        contestData.TryGetProperty("description", out property) ? property.GetString() : null,
        contestData.TryGetProperty("difficulty", out property) ? property.GetInt32() : null,
        contestData.TryGetProperty("kind", out property) ? property.GetString() : null,
        contestData.TryGetProperty("icpcRegion", out property) ? property.GetString() : null,
        contestData.TryGetProperty("country", out property) ? property.GetString() : null,
        contestData.TryGetProperty("city", out property) ? property.GetString() : null,
        contestData.TryGetProperty("season", out property) ? property.GetString() : null
    );

    private static CfProblem GetProblemFromData(JsonElement problemData) => new(
        problemData.GetProperty("index").GetString()!,
        Enum.Parse<CfProblemType>(problemData.GetProperty("type").GetString()!.Replace("_", ""), ignoreCase: true),
        problemData.GetProperty("name").GetString()!,
        problemData.GetProperty("tags").EnumerateArray().Select(e => e.GetString()!).ToList(),
        problemData.TryGetProperty("contestId", out var property) ? property.GetInt32() : null,
        problemData.TryGetProperty("problemsetName", out property) ? property.GetString() : null,
        problemData.TryGetProperty("points", out property) ? property.GetDouble() : null,
        problemData.TryGetProperty("rating", out property) ? property.GetInt32() : null
    );

    private static CfRankListRow GetRankListRowFromData(JsonElement rankListRowData) => new(
        GetPartyFromData(rankListRowData.GetProperty("party")),
        rankListRowData.GetProperty("rank").GetInt32(),
        rankListRowData.GetProperty("points").GetDouble(),
        rankListRowData.GetProperty("penalty").GetInt32(),
        rankListRowData.GetProperty("successfulHackCount").GetInt32(),
        rankListRowData.GetProperty("unsuccessfulHackCount").GetInt32(),
        rankListRowData.GetProperty("problemResults").EnumerateArray().Select(e => e.GetInt32()).ToList(),
        rankListRowData.GetProperty("lastSubmissionTimeSeconds").TryGetInt32(out var seconds) ? seconds : null
    );

    private static CfSubmission GetSubmissionFromData(JsonElement submissionData) => new(
        submissionData.GetProperty("id").GetInt32(),
        submissionData.GetProperty("creationTimeSeconds").GetInt32(),
        submissionData.GetProperty("relativeTimeSeconds").GetInt32(),
        GetProblemFromData(submissionData.GetProperty("problem")),
        GetPartyFromData(submissionData.GetProperty("author")),
        submissionData.GetProperty("programmingLanguage").GetString()!,
        submissionData.TryGetProperty("verdict", out var property)
            ? Enum.Parse<CfVerdict>(property.GetString()!.Replace("_", ""), ignoreCase: true) : null,
        Enum.Parse<CfTestset>(submissionData.GetProperty("testset").GetString()!.Replace("_", ""), ignoreCase: true),
        submissionData.GetProperty("passedTestCount").GetInt32(),
        submissionData.GetProperty("timeConsumedMillis").GetInt32(),
        submissionData.GetProperty("memoryConsumedBytes").GetInt32(),
        submissionData.TryGetProperty("points", out property) ? property.GetDouble() : null,
        submissionData.TryGetProperty("contestId", out property) ? property.GetInt32() : null
    );

    private static CfParty GetPartyFromData(JsonElement partyData) => new(
        partyData.GetProperty("members").EnumerateArray().Select(GetMemberFromData).ToList(),
        partyData.GetProperty("ghost").GetBoolean(),
        Enum.Parse<CfParticipantType>(partyData.GetProperty("participantType").GetString()!.Replace("_", ""),
            ignoreCase: true
        ),
        partyData.TryGetProperty("teamId", out var property) ? property.GetInt32() : null,
        partyData.TryGetProperty("teamName", out property) ? property.GetString() : null,
        partyData.TryGetProperty("room", out property) ? property.GetInt32() : null,
        partyData.TryGetProperty("contestId", out property) ? property.GetInt32() : null,
        partyData.TryGetProperty("startTimeSeconds", out property) ? property.GetInt32() : null
    );

    private static CfMember GetMemberFromData(JsonElement memberData) => new(
        memberData.GetProperty("handle").GetString()!,
        memberData.TryGetProperty("name", out var property) ? property.GetString() : null
    );
}
