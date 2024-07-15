using System.Reflection;
using CodeForcer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(
        builder.Configuration.GetConnectionString("CodeForcerDb") ??
        throw new ArgumentException("No connection string provided.")
    );
    
    builder.Services.AddCarter();
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    );
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapCarter();
    
    app.Run();
}
