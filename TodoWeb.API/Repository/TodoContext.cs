using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TodoWeb.API.Controllers;
using TodoWeb.API.Models;

namespace TodoWeb.API.Repository;

public interface ITodoPersistence
{
    public Task<Todo> Create(CreateTodo createTodo);
    public Task<List<Todo>> GetAll();
    public Task<Todo> GetById(Guid id);
    public Task<Todo> Update(UpdateTodo updateTodo);
    public Task DeleteById(Guid id);
    public Task DeleteAll();
}

public class TodoContext(DbContextOptions<TodoContext> options, ILogger<TodoContext> logger) 
    : DbContext(options), ITodoPersistence
{
    private readonly ILogger _logger = logger;
    public DbSet<Todo> Todos { get; set; }
    
    public async Task<Todo> Create(CreateTodo createTodo)
    {
        Debug.Assert(createTodo.Name != null);
        
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Name = createTodo.Name,
            Description = createTodo.Description,
            CreatedDate = DateTime.Now,
            Priority = Priority.Unclaimed,
            DueDate = createTodo.DueDate
        };
        
        await Todos.AddAsync(todo);
        await SaveChangesAsync();
        return todo;
    }

    public Task<List<Todo>> GetAll()
    {
        return Todos.ToListAsync();
    }

    public async Task<Todo> GetById(Guid id)
    {
        var todo = await Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            _logger.LogWarning("Getting Todo with ID: {id} was null." +
                              $"The todo will be null", id);
        }
        return todo;
    }

    public async Task<Todo> Update(UpdateTodo updateTodo)
    {
        var todo = await Todos
            .FirstOrDefaultAsync(t => t.Id == updateTodo.Id);
        if (todo == null)
        {
            _logger.LogWarning("Getting Todo with ID: {id} was null. The todo will be null", updateTodo.Id);
            return null;
        }

        Todos.Attach(todo);

        todo.Name = updateTodo?.Name ?? todo.Name;
        todo.Description = updateTodo?.Description ?? todo.Description;
        todo.Priority = updateTodo?.Priority ?? todo.Priority;
        todo.DueDate = updateTodo?.DueDate;
        todo.IsCompleted = updateTodo?.IsCompleted ?? todo.IsCompleted;

        await SaveChangesAsync();
        return todo;
    }

    public async Task DeleteById(Guid id)
    {
        var todo = new Todo { Id = id };
        Todos.Attach(todo);
        Todos.Remove(todo);
        await SaveChangesAsync();
    }

    public async Task DeleteAll()
    {
        Todos.RemoveRange(Todos);
        await SaveChangesAsync();
    }
}