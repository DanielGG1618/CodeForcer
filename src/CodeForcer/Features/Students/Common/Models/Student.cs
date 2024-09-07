using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeForcer.Features.Students.Common.Models;

[JsonConverter(typeof(StudentJsonConverter))]
public record Student(Email? Email, string Handle)
{
    public Email? Email { get; set; } = Email;

    public Student(string handle) : this(null, handle) {}
}

public class StudentJsonConverter : JsonConverter<Student>
{
    public override Student Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? email = null;
        string? handle = null;

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
                break;

            if (reader.TokenType is JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case nameof(Student.Email):
                        email = reader.TokenType is JsonTokenType.Null ? null
                            : reader.GetString();
                        break;

                    case nameof(Student.Handle):
                        handle = reader.GetString();
                        break;

                    default:
                        throw new SerializationException($"Unknown json property found: {propertyName}");
                }
            }
        }

        if (handle is null)
            throw new SerializationException($"{nameof(handle)} is null");

        return new Student(
            email is null ? null
                : Email.Create(email),
            handle
        );
    }

    public override void Write(Utf8JsonWriter writer, Student student, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (student.Email.HasValue)
            writer.WriteString(nameof(Student.Email), student.Email.Value);
        else
            writer.WriteNull(nameof(Student.Email));
        writer.WriteString(nameof(Student.Handle), student.Handle);
        writer.WriteEndObject();
    }
}
