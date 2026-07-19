using BootcampCLT2026.Application;
using BootcampCLT2026.Endpoints;
using BootcampCLT2026.Infraestructure;
using BootcampCLT2026.Infraestructure.Persistence;
using BootcampCLT2026.Middleware;
using Scalar.AspNetCore;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseValidationExceptionHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "BootcampCLT2026 API";
    });

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
}

app.MapCuentaEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();