using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var todos = new List<Todo>();
var nextID = 1;

app.MapGet("/todos", () => todos);
app.MapGet("/todos/{id}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todos", (Todo newTodo) =>
{
    newTodo.Id = nextID++;
    todos.Add(newTodo);
    return Results.Created("$/todos/{newTodo.id}", newTodo);
});

app.MapPut("/todos/{id}", (int id, Todo updatedTodo) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();

    todo.Title = updatedTodo.Title;
    todo.isComleted = updatedTodo.isComleted;
    return Results.Ok(todo);
});

app.MapDelete("todos/{id}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if(todo is null) return Results.NotFound();

    todos.Remove(todo);
    return Results.NoContent();
});

app.Run();

public class Todo
{
    public int Id{get; set;}
    public string Title{get; set;} = string.Empty;
    public bool isComleted{get; set;}
}
