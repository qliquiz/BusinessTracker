using BusinessTracker.Api.Logics;
using BusinessTracker.Api.Models;
using BusinessTracker.Api.Workers;
using BusinessTracker.Common.Core;
using BusinessTracker.Data.Extensions;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<ApiOptions>(configuration.GetSection(nameof(ApiOptions)));

var apiOptions = configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>();
var connectionString = apiOptions?.PostgresConnectionString
                       ?? configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Port=5433;Username=admin;Password=123456;Database=business_tracker";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "BusinessTrackerApi_.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7
    )
    .CreateLogger();

// builder.Host.UseSerilog();

DatabaseMigrator.Migrate(connectionString);

builder.Services.RegisterBusinessTrackerData(connectionString);
builder.Services.AddScoped<ILoadingService, LoadingService>();
builder.Services.AddHostedService<JournalNormalizationWorker>();
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