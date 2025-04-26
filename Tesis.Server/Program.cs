using Tesis.Application.Services.Evaluacion;
using Tesis.DataAcces.Repository.IRepository;
using Tesis.DataAcces.Repository;
using Microsoft.EntityFrameworkCore;
using Tesis.DataAcces;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SeriLogThemesLibrary;

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(theme: SeriLogCustomThemes.Theme1())
    .WriteTo.File("logs/api-log.txt")
    .CreateLogger();



var builder = WebApplication.CreateBuilder(args);

//SeriLog
builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// UnitOfWorks Service
builder.Services.AddScoped<IUnitOfWorks, UnitOfWorks>();
//Servicio de Evaluacion
builder.Services.AddScoped<IEvaluador, Evaluador>();
//Database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

Log.Information("API iniciada en {StartupTime}", DateTime.UtcNow);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(); // Registra detalles de cada solicitud HTTP

app.UseAuthorization();

app.MapControllers();

app.Run();
