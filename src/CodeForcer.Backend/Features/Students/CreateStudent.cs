using AutoApiGen.Attributes;
using CodeForcer.Backend.Features.Students.Common.Interfaces;
using CodeForcer.Backend.Features.Students.Common.Models;

namespace CodeForcer.Backend.Features.Students;

public static class CreateStudent
{
    [PostEndpoint("students",
        SuccessCode = StatusCodes.Status201Created,
        ErrorCode = StatusCodes.Status400BadRequest
    )]
    public record Command(string Email, string Handle) : IRequest<ErrorOr<Student>>;

    public class CommandHandler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Command, ErrorOr<Student>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async ValueTask<ErrorOr<Student>> Handle(Command command, CancellationToken cancellationToken)
        {
            var (email, handle) = command;

            return await Email.SafeCreate(email)
                .Then(value => new Student(value, handle))
                .ThenDoAsync(_studentsRepository.Add);
        }
    }
}
