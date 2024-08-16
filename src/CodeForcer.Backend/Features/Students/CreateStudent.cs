using CodeForcer.Common.Models;
using CodeForcer.Contracts;
using CodeForcer.Features.Students.Common.Domain;
using CodeForcer.Features.Students.Common.Extensions;
using CodeForcer.Features.Students.Common.Interfaces;

namespace CodeForcer.Features.Students;

public static class CreateStudent
{
    public record Command(string Email, string Handle) : IRequest<ErrorOr<Student>>;

    public class Endpoint : EndpointBase
    {
        public override void AddRoutes(IEndpointRouteBuilder app) => app.MapPost("students",
            async (CreateStudentRequest request, ISender mediator) =>
            {
                var command = new Command(request.Email, request.Handle);

                var result = await mediator.Send(command);

                return result.Match(
                    student => Created($"student/{student.Email}", student.ToResponse()),
                    errors => Problem(errors)
                );
            }
        );
    }

    public class CommandHandler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Command, ErrorOr<Student>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public Task<ErrorOr<Student>> Handle(Command command, CancellationToken cancellationToken)
        {
            var (email, handle) = command;

            var errorOrStudent = Student.SafeCreate(email, handle)
                .ThenDoAsync(_studentsRepository.Add);

            return errorOrStudent;
        }
    }
}
