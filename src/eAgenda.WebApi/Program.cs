var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Endpoint de API (Application Programming Interface)
// Minimal API
app.MapGet("/", () => "Hello World!");

app.Run();
