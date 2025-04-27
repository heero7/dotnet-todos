using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Todo.API;
using Todo.API.Controllers;
using Todo.API.Repository;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    // todo: this isn't working
    .WriteTo.File($"/logs/Todo_WebApi_{DateTime.Now}.log")
    .CreateLogger();

// Configure Logging using SeriLog
builder.Host.UseSerilog();

// Setup for MVC
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(opt =>
    {
        /*
         * Adds the logging when hitting any validation
         * issues. Without this, developers would never
         * see the logs when hitting validation errors.
         */
        opt.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Validation Error hitting this endpoint. {errors}",
                context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new BadRequestObjectResult(context.ModelState);
        };
    });

// Add documentation and interaction w/o postman.
builder.Services.AddSwaggerGen();

// Setup databases.
    // todo: this should be abstracted!
    // how can we abstract the details here, this is directly tied to Entity Framework
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoPersistence, TodoContext>();

builder.Services.AddProblemDetails();

// Custom Exception Handling
builder.Services.AddExceptionHandler<TodoGlobalExceptionHandler>();

// Add services to the container.
var app = builder.Build();

/*
 * Reduces exception hell. Place this as close
 * to the Build method so it catches exceptions
 * early. (i.e. if you had other middleware)
 * Without this, you'd have to write code like
 * try {
 *  someService.Foo();
 * } catch (Exception e) {
 *  ...
 * }
 * Everywhere, maps an exception to a status code
 */
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();

app.Run();
