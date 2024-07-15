using System.ComponentModel.DataAnnotations;
using CodeForcer.Features.Students.Common;
using CodeForcer.Features.Students.Common.Extensions;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Features.Students;

public static class GetStudent
{
    public record Command(string EmailOrHandle) : IRequest<ErrorOr<Student>>;

    public class Endpoint : EndpointBase
    {
        public override void AddRoutes(IEndpointRouteBuilder app) => app.MapGet("students/{emailOrHandle}",
            async (string emailOrHandle, ISender mediatr) =>
            {
                var command = new Command(emailOrHandle);

                var result = await mediatr.Send(command);

                return result.Match(
                    student => Ok(student.ToResponse()),
                    errors => Problem(errors)
                );
            });
    }

    public class CommandHandler(
        IStudentsRepository studentsRepository
    ) : IRequestHandler<Command, ErrorOr<Student>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;
        
        public async Task<ErrorOr<Student>> Handle(Command command, CancellationToken cancellationToken)
        {
            var emailOrHandle = command.EmailOrHandle;
            
            var student = new EmailAddressAttribute().IsValid(emailOrHandle)
                ? await _studentsRepository.GetByEmail(emailOrHandle)
                : await _studentsRepository.GetByHandle(emailOrHandle);

            return student is null 
                ? StudentsErrors.NotFound 
                : student;
        }
    }
}