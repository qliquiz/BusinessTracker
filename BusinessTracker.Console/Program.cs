using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using BusinessTracker.Console.Models;
using BusinessTracker.Domain.Models.Dto;
using BusinessTracker.Domain;

CurrentApplication.ShowLogo();

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var config = builder.Build();

var options = config.Get<ApplicationOptions>() ??
              throw new InvalidOperationException("Unabled loading appsettings.json");

await using var connection = new SqlConnection(options.ConnectionString);
connection.Open();

const string query = "SELECT TOP 10 * FROM journal";
var command = new SqlCommand(query, connection);

// Способ 1
/*var reader = command.ExecuteReader();
var position = 1;

while (reader.Read())
{
    Console.WriteLine($"{position} - {reader.GetInt32(0)}, {reader.GetInt32(1)}");
    position++;
}*/

// Способ 2
var adapter = new SqlDataAdapter(command);
var dataset = new DataSet();
adapter.Fill(dataset);

var table = dataset.Tables[0];
for (var position = 0; position < table.Rows.Count; position++)
{
    // Создаем доменную модель
    var dto = new JournalRowDto
    {
        Period = Convert.ToDateTime(table.Rows[position]["dater"]),
        Quantity = Convert.ToDouble(table.Rows[position]["quantity"]),
        Price = Convert.ToDouble(table.Rows[position]["price"])
    };

    Console.WriteLine(dto);
}