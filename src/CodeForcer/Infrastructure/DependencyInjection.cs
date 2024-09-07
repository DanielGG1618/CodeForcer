using CodeForcer.Features.Contests.Common.Interfaces;
using CodeForcer.Features.Students.Common.Interfaces;
using CodeForcer.Infrastructure.CodeForces;

namespace CodeForcer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string dbConnectionString
    ) => services.AddSingleton<IStudentsRepository>(_ =>
            new StudentsRepository(dbConnectionString)
        ).AddTransient<IHandleValidator, CodeForcesContestsProvider>()
        .AddTransient<IContestsProvider, CodeForcesContestsProvider>();
}
