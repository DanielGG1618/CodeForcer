using AutoApiGen.Attributes;
using CodeForcer.Backend.Features.Students.Common;
using CodeForcer.Backend.Features.Students.Common.Interfaces;
using CodeForcer.Backend.Features.Students.Common.Models;

namespace CodeForcer.Backend.Features.Students;

public static class UpdateOrCreateStudent
{
    [PutEndpoint("students/{EmailOrHandle}",
        SuccessCode = StatusCodes.Status204NoContent,
        ErrorCode = StatusCodes.Status400BadRequest
    )]
    public record Command(string EmailOrHandle, string Email, string Handle) : ICommand<ErrorOr<Success>>;

    public class Handler(
        IStudentsRepository studentsRepository
    ) : ICommandHandler<Command, ErrorOr<Success>>
    {
        private readonly IStudentsRepository _studentsRepository = studentsRepository;

        public async ValueTask<ErrorOr<Success>> Handle(Command request, CancellationToken cancellationToken)
        {
            var (emailOrHandle, emailStr, handle) = request;

            if (!Email.IsValid(emailStr, out var email))
                return StudentsErrors.InvalidEmail;

            var student = new Student(email, handle);

            if (Email.IsValid(emailOrHandle))
            {
                if (emailOrHandle != emailStr)
                    return StudentsErrors.EmailsDoesNotMatch;

                if (await _studentsRepository.ExistsByEmail(email))
                    await _studentsRepository.UpdateByEmail(email, student);
                else
                    await _studentsRepository.Add(student);
            }
            else
            {
                if (emailOrHandle != handle)
                    return StudentsErrors.HandlesDoesNotMatch;

                if (await _studentsRepository.ExistsByHandle(handle))
                    await _studentsRepository.UpdateByHandle(handle, student);
                else
                    await _studentsRepository.Add(student);
            }

            return Result.Success;
        }
    }
}
