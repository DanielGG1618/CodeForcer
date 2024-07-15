using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Features.Students.Domain;

namespace CodeForcer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string dbConnectionString
    ) => services.AddSingleton<IStudentsRepository>(_ => new StudentsRepository(dbConnectionString));
}