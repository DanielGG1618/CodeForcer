using CodeForcer.Common.Models;
using CodeForcer.Features.Students.Common.Domain;
using CodeForcer.Features.Students.Common.Extensions;
using CodeForcer.Features.Students.Common.Interfaces;

namespace CodeForcer.Features.Students;

public static class GetAllStudents
{
    public record Query : IRequest<ErrorOr<IEnumerable<Student>>>;

    public class Endpoint : EndpointBase
    {
        public override void AddRoutes(IEndpointRouteBuilder app) => app.MapGet("students/",
            async (ISender mediatr) =>
            {
                var command = new Query();

                var result = await mediatr.Send(command);

                return result.Match(
                    students => Ok(students.Select(s => s.ToResponse())),
                    errors => Problem(errors)
                );
            }
        );
    }

    public class QueryHandler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Query, ErrorOr<IEnumerable<Student>>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async Task<ErrorOr<IEnumerable<Student>>> Handle(Query command, CancellationToken cancellationToken) =>
            (await _studentsRepository.GetAll()).ToErrorOr();
    }
}
