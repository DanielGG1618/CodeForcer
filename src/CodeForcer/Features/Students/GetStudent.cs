using AutoApiGen.Attributes;
using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Features.Students;

public static class GetStudent
{
    [GetEndpoint("students/{EmailOrHandle}",
        ErrorCode = StatusCodes.Status404NotFound
    )]
    public record Query(string EmailOrHandle) : IRequest<ErrorOr<Student>>;

    public class CommandHandler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Query, ErrorOr<Student>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async ValueTask<ErrorOr<Student>> Handle(Query query, CancellationToken cancellationToken)
        {
            var emailOrHandle = query.EmailOrHandle;

            var student = Email.IsValid(emailOrHandle, out var email)
                ? await _studentsRepository.GetByEmail(email)
                : await _studentsRepository.GetByHandle(emailOrHandle);

            return student is null
                ? StudentsErrors.NotFound
                : student;
        }
    }
}
