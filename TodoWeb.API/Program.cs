using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using TodoWeb.API.Controllers;
using TodoWeb.API.Repository;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"/logs/Todo_WebApi_{DateTime.Now}.log")
    .CreateLogger();

// Configure Logging using SeriLog
builder.Host.UseSerilog();

// Setup for MVC
builder.Services.AddControllers();

// Add documentation and interaction w/o postman.
builder.Services.AddSwaggerGen();

// Setup databases.
    // todo: this should be abstracted!
    // how can we abstract the details here, this is directly tied to Entity Framework
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoPersistence, TodoContext>();

// Add services to the container.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*
 * Reduces exception hell.
 * Without this, you'd have to write code like
 * try {
 *  someService.Foo();
 * } catch (Exception e) {
 *  ...
 * }
 * Everywhere, maps an exception to a status code
 */
app.UseExceptionHandler(config =>
{
    config.Run(async cxt =>
    {
        var exception = cxt.Features.Get<IExceptionHandlerFeature>()?.Error;
        var logger = cxt.RequestServices.GetRequiredService<ILogger<Program>>();
        await cxt.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "You got an error. Sorry!" }));
    });
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
