using System.Reflection;
using BusinessTracker.Data;
using DbUp;

const string connectionString = "Host=localhost;Port=5433;Username=admin;Password=123456;Database=business_tracker";

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