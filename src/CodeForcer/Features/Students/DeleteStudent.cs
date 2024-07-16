using CodeForcer.Common.Models;
using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Interfaces;

namespace CodeForcer.Features.Students;

public static class DeleteStudent
{
    public record Command(string Email) : IRequest<ErrorOr<Deleted>>;

    public sealed class Endpoint : EndpointBase
    {
        public override void AddRoutes(IEndpointRouteBuilder app) => app.MapDelete("students/{email}",
            async (string email, ISender mediator) =>
            {
                var command = new Command(email);

                var result = await mediator.Send(command);

                return result.Match(
                    _ => NoContent(),
                    errors => Problem(errors)
                );
            });
    }

    public sealed class Handler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Command, ErrorOr<Deleted>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async Task<ErrorOr<Deleted>> Handle(Command request, CancellationToken cancellationToken)
        {
            var email = request.Email;

            var deleted = await _studentsRepository.DeleteByEmail(email);

            return deleted
                ? Result.Deleted
                : StudentsErrors.NotFound;
        }
    }
}