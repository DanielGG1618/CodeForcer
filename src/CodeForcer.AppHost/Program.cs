var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CodeForcer_Backend>("codeforcer-backend");

var application = builder.Build();

application.Run();
