using CodeForcer.Features.Students.Common.Extensions;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Features.Students;

public static class GetAllStudents
{
    public record Command : IRequest<ErrorOr<IEnumerable<Student>>>;

    public class Endpoint : EndpointBase
    {
        public override void AddRoutes(IEndpointRouteBuilder app) => app.MapGet("students/",
            async (ISender mediatr) =>
            {
                var command = new Command();

                var result = await mediatr.Send(command);

                return result.Match(
                    students => Ok(students.Select(s => s.ToResponse())),
                    errors => Problem(errors)
                );
            });
    }

    public class CommandHandler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Command, ErrorOr<IEnumerable<Student>>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async Task<ErrorOr<IEnumerable<Student>>> Handle(Command command, CancellationToken cancellationToken)
        {
            var students = await _studentsRepository.GetAll();

            return students.ToErrorOr();
        }
    }
}