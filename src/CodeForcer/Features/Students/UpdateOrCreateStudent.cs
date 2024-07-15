using CodeForcer.Common.Models;
using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Domain;
using CodeForcer.Features.Students.Common.Extensions;
using CodeForcer.Features.Students.Common.Interfaces;

namespace CodeForcer.Features.Students;

public static class UpdateOrCreateStudent
{
    public record Command(string Email, string Handle) : IRequest<ErrorOr<Student?>>;

    public class Endpoint : EndpointBase
    {
        public override void AddRoutes(IEndpointRouteBuilder app) => app.MapPut("students/{email}",
            async (string email, UpdateOrCreateStudentRequest request, ISender mediatr) =>
            {
                if (email != request.Email)
                    return Problem(StudentsErrors.EmailsDoesNotMatch);

                var command = new Command(request.Email, request.Handle);

                var result = await mediatr.Send(command);

                return result.Match(
                    student => student is null
                        ? NoContent()
                        : Created($"students/{email}", student.ToResponse()),
                    errors => Problem(errors)
                );
            });
    }

    public class Handler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Command, ErrorOr<Student?>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async Task<ErrorOr<Student?>> Handle(Command request, CancellationToken cancellationToken)
        {
            var (email, handle) = request;

            if (await _studentsRepository.ExistsByEmail(email))
            {
                var errorOrNull = await Student.SafeCreate(email, handle)
                    .ThenDoAsync(s => _studentsRepository.UpdateByEmail(email, s))
                    .Then(_ => (Student?)null);

                return errorOrNull;
            }

            var errorOrStudent = await Student.SafeCreate(email, handle)
                .ThenDoAsync(_studentsRepository.Add);

            return errorOrStudent!;
        }
    }
}

public record UpdateOrCreateStudentRequest(string Email, string Handle);