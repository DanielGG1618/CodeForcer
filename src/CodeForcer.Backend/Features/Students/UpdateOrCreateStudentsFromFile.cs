using Microsoft.AspNetCore.Mvc;

namespace CodeForcer.Features.Students;

[Route("students")]
public class StudentsController : ControllerBase
{
    [HttpPost("file")]
    public IActionResult UpdateOrCreateStudentsFromFile(ISender mediator, IFormFile file)
    {
        mediator.Send(new GetStudent.Command(""));
        return Ok(file.FileName);
    }
}

/*
 * public static class Feature
 * {
 *      public record Command(
 *          [FromRoute] string Email,
 *          [FromQuery] string Handle,
 *          [FromForm] IFormFile File
 *      ) : IRequest<ErrorOr<Student?>>;
 *
 *      [Endpoint<StudentsController>("students/{email}")]
 *      public class Handler(
 *          IStudentsRepository studentsRepository
 *      ) : IRequestHandler<Command, ErrorOr<Student>>
 *      {
 *          private readonly IStudentsRepository _studentsRepository = studentsRepository;
 *
 *          public async Task<ErrorOr<Student> Handle(Command command, CancellationToken cancellationToken)
 *          {
 *              var (email, file) = command;
 *
 *              #commented some random code
 *
 *              return something;
 *          }
 *      }
 * }
 *
 */
