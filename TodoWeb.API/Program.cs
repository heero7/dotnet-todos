using Microsoft.EntityFrameworkCore;
using TodoWeb.API.Controllers;
using TodoWeb.API.Repository;

var builder = WebApplication.CreateBuilder(args);

// Setup for MVC
builder.Services.AddControllers();

// Setup databases.
    // todo: this should be abstracted!
    // how can we abstract the details here, this is directly tied to Entity Framework
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoPersistence, TodoContext>();

// Add services to the container.
var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
