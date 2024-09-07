using CodeForcer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(
        builder.Configuration.GetConnectionString("CodeForcerDb")
        ?? throw new ArgumentException("No connection string provided.")
    );

    builder.Services.AddControllers();
    builder.Services.AddMediator();

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

    app.MapControllers();

    app.Run();
}
