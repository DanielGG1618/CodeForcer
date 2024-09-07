using AutoApiGen.Attributes;
using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Features.Students;

public static class DeleteStudent
{
    [DeleteEndpoint("students/{Email}",
        SuccessCode = StatusCodes.Status204NoContent,
        ErrorCode = StatusCodes.Status404NotFound
    )]
    public record Command(string Email) : IRequest<ErrorOr<Deleted>>;

    public sealed class Handler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Command, ErrorOr<Deleted>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async ValueTask<ErrorOr<Deleted>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!Email.IsValid(request.Email, out var email))
                return StudentsErrors.InvalidEmail;

            var deleted = await _studentsRepository.DeleteByEmail(email);

            return deleted ? Result.Deleted : StudentsErrors.NotFound;
        }
    }
}
