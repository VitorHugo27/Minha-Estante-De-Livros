using Microsoft.Extensions.Options;
using MinhaEstante.Application;
using MinhaEstante.Infrastructure;
using System.Text.Json.Serialization;
using MinhaEstante.Api.Handlers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MinhaEstante")
    ?? throw new InvalidOperationException("Connection string 'MinhaEstante' não encontrada.");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
       options.AddPolicy("Angular", policy =>
           policy.WithOrigins("http://localhost:4200")
                 .AllowAnyHeader()
                 .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.Services.AplicarMigrations();
}

app.UseCors("Angular");
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "MinhaEstante"));
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

app.Run();
