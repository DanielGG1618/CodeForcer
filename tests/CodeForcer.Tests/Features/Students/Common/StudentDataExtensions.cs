using CodeForcer.Features.Students.Common.Models;

namespace CodeForcer.Tests.Features.Students.Common;

public static class StudentDataExtensions
{
    public static IEnumerable<Student> ToDomain(this IEnumerable<StudentData> studentDatas) =>
        studentDatas.Select(static s => s.ToDomain());
}
