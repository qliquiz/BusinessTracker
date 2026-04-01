using System.Reflection;
using Scalar.AspNetCore;
using BusinessTracker.Api.Logics;
using BusinessTracker.Api.Models;
using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Data.Extensions;
using DbUp;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Настройки приложения через DI
builder.Services.Configure<ApiOptions>(configuration.GetSection(nameof(ApiOptions)));

var apiOptions = configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>();
var connectionString = apiOptions?.PostgresConnectionString
                       ?? configuration.GetConnectionString("DefaultConnection")
                       ?? "User ID=admin;Password=123456;Host=localhost;Port=5433;Database=business_tracker;";

// DbUp: применяем миграции при старте
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
builder.Services.RegisterBusinessTrackerData(connectionString);
builder.Services.AddScoped<ILoadingService, LoadingService>();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.WebHost.UseUrls("http://0.0.0.0:8000");

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseRouting();
app.MapControllers();

app.Run();