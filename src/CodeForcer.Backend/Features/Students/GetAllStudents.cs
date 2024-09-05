using AutoApiGen.Attributes;
using CodeForcer.Backend.Features.Students.Common.Interfaces;
using CodeForcer.Backend.Features.Students.Common.Models;

namespace CodeForcer.Backend.Features.Students;

public static class GetAllStudents
{
    [GetEndpoint("students")]
    public record Query : IQuery<ErrorOr<List<Student>>>;

    public class Handler(
        IStudentsRepository studentsRepository
    ) : IQueryHandler<Query, ErrorOr<List<Student>>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async ValueTask<ErrorOr<List<Student>>> Handle(
            Query query,
            CancellationToken cancellationToken
        ) => (await _studentsRepository.GetAll()).ToErrorOr();
    }
}
