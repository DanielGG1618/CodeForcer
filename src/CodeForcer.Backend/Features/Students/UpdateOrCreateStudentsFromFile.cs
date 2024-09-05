using System.Globalization;
using AutoApiGen.Attributes;
using CodeForcer.Backend.Features.Students.Common;
using CodeForcer.Backend.Features.Students.Common.Interfaces;
using CodeForcer.Backend.Features.Students.Common.Models;
using CsvHelper;

namespace CodeForcer.Backend.Features.Students;

public static class UpdateOrCreateStudentsFromFile
{
    [PostEndpoint("students/file",
        ErrorCode = StatusCodes.Status400BadRequest
    )]
    public sealed record Command(IFormFile File) : ICommand<ErrorOr<Response>>;

    public sealed record Response(int Updated, int Created);

    public sealed class Handler(
        IStudentsRepository studentsRepository
    ) : ICommandHandler<Command, ErrorOr<Response>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async ValueTask<ErrorOr<Response>> Handle(Command command, CancellationToken cancellationToken)
        {
            var file = command.File;

            await using var stream = file.OpenReadStream();

            using var csv = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);

            var updated = 0;
            var created = 0;
            await foreach (var studentRecord in csv.GetRecordsAsync(new { email = "", handle = "" }, cancellationToken))
            {
                var (emailStr, handle) = (studentRecord.email, studentRecord.handle);

                if (!Email.IsValid(emailStr, out var email))
                    return StudentsErrors.InvalidEmail;

                var student = new Student(email, handle);

                if (await _studentsRepository.ExistsByEmail(email))
                {
                    await _studentsRepository.UpdateByEmail(email, student);
                    updated++;
                }
                else
                {
                    await _studentsRepository.Add(student);
                    created++;
                }
            }

            return new Response(updated, created);
        }
    }
}
