using MyApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var nameService = new NameService();
app.MapGet("/", () => "Hello World!");

app.MapGet("/users", () => new[] 
{
    new { Name = "Alice", Age = 25 },
    new { Name = "Bob", Age = 30 }
});

app.MapGet("/users/{name}", (string name) => 
{
    var result = nameService.GetName(name);
    return $"Привет, {result}!";
});
app.Run();