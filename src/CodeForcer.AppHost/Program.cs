var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CodeForcer>("codeforcer");

var application = builder.Build();

application.Run();
