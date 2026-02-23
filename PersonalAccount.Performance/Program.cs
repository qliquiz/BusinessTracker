using System.Data;
using System.Diagnostics;
using BusinessTracker.Domain.Logic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = builder.Build();
var connectionString = config.GetConnectionString("DefaultConnection") ?? config["ConnectionString"] ??
    throw new InvalidOperationException("Connection string not found.");

Console.WriteLine("Запуск тестов производительности загрузки транзакций...");

await LoadForDay();
await LoadForMonth();
await LoadForQuarter();
return;

async Task LoadForDay()
{
    Console.WriteLine("\n--- Загрузка транзакций за день ---");
    var referenceDate = new DateTime(2023, 11, 15);
    var today = referenceDate;
    var tomorrow = referenceDate.AddDays(1);
    await ExecuteAndMeasure(today, tomorrow);
}

async Task LoadForMonth()
{
    Console.WriteLine("\n--- Загрузка транзакций за месяц ---");
    var referenceDate = new DateTime(2023, 11, 15);
    var firstDayOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
    var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
    await ExecuteAndMeasure(firstDayOfMonth, firstDayOfNextMonth);
}

async Task LoadForQuarter()
{
    Console.WriteLine("\n--- Загрузка транзакций за квартал ---");
    var referenceDate = new DateTime(2023, 11, 15);
    var quarter = (referenceDate.Month - 1) / 3 + 1;
    var firstDayOfQuarter = new DateTime(referenceDate.Year, (quarter - 1) * 3 + 1, 1);
    var firstDayOfNextQuarter = firstDayOfQuarter.AddMonths(3);
    await ExecuteAndMeasure(firstDayOfQuarter, firstDayOfNextQuarter);
}

async Task ExecuteAndMeasure(DateTime startDate, DateTime endDate)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    try
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var transTypes = await LoadTransactionTypes(connection);

        const string query = "SELECT * FROM journal WHERE dater >= @startDate AND dater < @endDate";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@startDate", startDate);
        command.Parameters.AddWithValue("@endDate", endDate);

        var adapter = new SqlDataAdapter(command);
        var dataSet = new DataSet();
        await Task.Run(() => adapter.Fill(dataSet));
        var journalTable = dataSet.Tables[0];

        Console.WriteLine($"Найдено {journalTable.Rows.Count} записей.");

        var transactions = DataMapper.LoadJournalTransactions(journalTable, transTypes);
        Console.WriteLine(transactions);

        stopwatch.Stop();

        Console.WriteLine($"Загрузка и маппинг завершены за: {stopwatch.ElapsedMilliseconds} мс.");
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Ошибка выполнения: {ex.Message}");
        Console.ResetColor();
    }
}

async Task<Dictionary<int, string>> LoadTransactionTypes(SqlConnection connection)

{
    var transTypes = new Dictionary<int, string>();

    var command = new SqlCommand("SELECT transtypeid, description FROM transtype", connection);

    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())

    {
        var id = reader.GetInt32(0);

        var description = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

        transTypes[id] = description;
    }

    return transTypes;
}