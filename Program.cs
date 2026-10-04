using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", async () =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection");

    var html = """
    <html>
    <head>
        <title>Customer Database Demo</title>
        <style>
            body { font-family: Arial; margin: 40px; }
            table { border-collapse: collapse; width: 700px; }
            th, td { border: 1px solid #ccc; padding: 10px; }
            th { background: #eee; }
        </style>
    </head>
    <body>
        <h1>Customer Database Demo</h1>
        <p>Data loaded from Azure SQL Database.</p>
        <table>
        <tr><th>ID</th><th>Name</th><th>Email</th></tr>
    """;

    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();

    var command = new SqlCommand(
        "SELECT Id, Name, Email FROM Customers",
        connection);

    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        html += $"<tr><td>{reader.GetInt32(0)}</td><td>{reader.GetString(1)}</td><td>{reader.GetString(2)}</td></tr>";
    }

    html += """
        </table>
    </body>
    </html>
    """;

    return Results.Content(html, "text/html");
});

app.Run();
