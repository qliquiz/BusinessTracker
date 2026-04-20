using BusinessTracker.Api.Logics;
using Microsoft.EntityFrameworkCore;
using BusinessTracker.Api.Models;
using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Data.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<ApiOptions>(configuration.GetSection(nameof(ApiOptions)));

var apiOptions = configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>();
var connectionString = apiOptions?.PostgresConnectionString
                       ?? configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Port=5433;Username=admin;Password=123456;Database=business_tracker";

builder.Services.RegisterBusinessTrackerData(connectionString);
builder.Services.AddScoped<ILoadingService, LoadingService>();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.WebHost.UseUrls("http://0.0.0.0:8000");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BusinessTrackerContext>();
    db.Database.Migrate();
}

app.UseDeveloperExceptionPage();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseRouting();
app.MapControllers();

app.Run();
