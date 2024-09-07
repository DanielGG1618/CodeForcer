using AutoApiGen.Attributes;
using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Features.Students;

public static class CreateStudent
{
    [PostEndpoint("students",
        SuccessCode = StatusCodes.Status201Created,
        ErrorCode = StatusCodes.Status400BadRequest
    )]
    public record Command(string Email, string Handle) : IRequest<ErrorOr<Student>>;

    public class CommandHandler(
        IStudentsRepository studentsRepository,
        IHandleValidator handleValidator
    ) : IRequestHandler<Command, ErrorOr<Student>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;
        private readonly IHandleValidator _handleValidator = handleValidator;

        public async ValueTask<ErrorOr<Student>> Handle(Command command, CancellationToken cancellationToken)
        {
            var (emailStr, handle) = command;

            if (!Email.IsValid(emailStr, out var email))
                return StudentsErrors.InvalidEmail;

            if (!await _handleValidator.IsValid(handle))
                return StudentsErrors.InvalidHandle;

            var student = new Student(email, handle);
            await _studentsRepository.Add(student);

            return student;
        }
    }
}
