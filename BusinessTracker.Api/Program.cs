using System.Reflection;
using BusinessTracker.Api.Logics;
using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Data.Extensions;
using DbUp;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// DbUp: применяем миграции при старте
var connectionString = configuration.GetConnectionString("DefaultConnection")!;
var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetAssembly(typeof(DataMarker)))
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();
if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
}

// DI
builder.Services.RegisterBusinessTrackerData(configuration);
builder.Services.AddScoped<ILoadingService, LoadingService>();
builder.Services.AddControllers();

builder.WebHost.UseUrls("http://0.0.0.0:8000");

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseRouting();
app.MapControllers();

app.Run();
