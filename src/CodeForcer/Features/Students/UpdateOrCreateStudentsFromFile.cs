using System.Globalization;
using AutoApiGen.Attributes;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Common.Models;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;

namespace CodeForcer.Features.Students;

public static class UpdateOrCreateStudentsFromFile
{
    [PostEndpoint("students/file",
        ErrorCode = StatusCodes.Status400BadRequest
    )]
    public sealed record Command(IFormFile File, [FromQuery] bool UseHandleAsKey = false) : ICommand<ErrorOr<Response>>;

    public sealed record Response(int Updated, int Created, List<string> InvalidEmails, List<string> InvalidHandles);

    public sealed class Handler(
        IStudentsRepository studentsRepository,
        IHandleValidator handleValidator
    ) : ICommandHandler<Command, ErrorOr<Response>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;
        private readonly IHandleValidator _handleValidator = handleValidator;

        public async ValueTask<ErrorOr<Response>> Handle(Command command, CancellationToken cancellationToken)
        {
            var (file, useHandleAsKey) = command;

            await using var stream = file.OpenReadStream();

            using var csv = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);

            var updated = 0;
            var created = 0;
            var invalidEmails = new List<string>();
            var invalidHandles = new List<string>();
            await foreach (var studentRecord in csv.GetRecordsAsync(new { email = "", handle = "" }, cancellationToken))
            {
                var (emailStr, handle) = (studentRecord.email, studentRecord.handle);

                if (!Email.IsValid(emailStr, out var email))
                {
                    invalidEmails.Add(emailStr);
                    continue;
                }

                if (!await _handleValidator.IsValid(handle))
                {
                    invalidHandles.Add(handle);
                    continue;
                }

                var student = new Student(email, handle);

                switch (useHandleAsKey)
                {
                    case false when await _studentsRepository.ExistsByEmail(email):
                        await _studentsRepository.UpdateByEmail(email, student);
                        updated++;
                        break;

                    case true when await _studentsRepository.ExistsByHandle(handle):
                        await _studentsRepository.UpdateByHandle(handle, student);
                        updated++;
                        break;

                    default:
                        await _studentsRepository.Add(student);
                        created++;
                        break;
                }
            }

            return new Response(updated, created, invalidEmails, invalidHandles);
        }
    }
}
