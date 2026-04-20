using BusinessTracker.Console.Extensions;
using BusinessTracker.Domain;
using Microsoft.Extensions.Hosting;

CurrentApplication.ShowLogo();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => { services.RegisterConsoleServices(context.Configuration); })
    .Build();

await host.RunAsync();