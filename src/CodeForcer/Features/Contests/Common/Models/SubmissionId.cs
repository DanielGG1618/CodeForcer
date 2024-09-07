using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeForcer.Features.Contests.Common.Models;

[JsonConverter(typeof(SubmissionIdJsonConverter))]
public readonly record struct SubmissionId(int Value);

public class SubmissionIdJsonConverter : JsonConverter<SubmissionId>
{
    public override SubmissionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.Number
            || !reader.TryGetInt32(out var value))
            throw new JsonException("Expected integer value.");

        return new SubmissionId(value);
    }

    public override void Write(Utf8JsonWriter writer, SubmissionId value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}
