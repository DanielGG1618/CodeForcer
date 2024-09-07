using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeForcer.Features.Contests.Common.Models;

[JsonConverter(typeof(ContestIdJsonConverter))]
public readonly record struct ContestId(int Value)
{
    public static implicit operator int(ContestId id) => id.Value;
}

public class ContestIdJsonConverter : JsonConverter<ContestId>
{
    public override ContestId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.Number
            || !reader.TryGetInt32(out var value))
            throw new JsonException("Expected integer value.");

        return new ContestId(value);
    }

    public override void Write(Utf8JsonWriter writer, ContestId value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}
