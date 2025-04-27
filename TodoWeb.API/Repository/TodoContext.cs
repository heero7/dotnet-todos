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
    public Task SoftDeleteById(Guid id);
    public Task SoftDeleteAll();
    public Task<Todo> DeleteById(Guid id);
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
        
        //todo: check if we've already created a todo with that name
        var previousTodo = await Todos.FirstOrDefaultAsync(t => t.Name == createTodo.Name);
        if (previousTodo != null)
        {
            _logger.LogError("A todo with this name already exists for this account.");
            return null;
        }
        
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
        var existingTodo = await Todos
            .FirstOrDefaultAsync(t => t.Id == updateTodo.Id);
        if (existingTodo == null)
        {
            _logger.LogWarning("Getting Todo with ID: {id} was null. The todo will be null", updateTodo.Id);
            return null;
        }
        

        Todos.Attach(existingTodo);

        if (string.Equals(existingTodo.Name, updateTodo.Name))
        {
            _logger.LogError("A todo with this name already exists for this account.");
        }
        else
        {
            existingTodo.Name = updateTodo.Name ?? existingTodo.Name;
        }
        existingTodo.Description = updateTodo.Description ?? existingTodo.Description;
        existingTodo.Priority = updateTodo.Priority ?? existingTodo.Priority;
        existingTodo.DueDate = updateTodo.DueDate ?? existingTodo.DueDate;
        existingTodo.IsCompleted = updateTodo.IsCompleted ?? existingTodo.IsCompleted;

        await SaveChangesAsync();
        return existingTodo;
    }

    public async Task SoftDeleteById(Guid id)
    {
        var todo = await Todos
            .FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            _logger.LogWarning("Getting Todo with ID: {id} was null. The todo will be null", id);
            return;
        }

        Todos.Attach(todo);
        todo.DeletedAt = DateTime.Now;
        await SaveChangesAsync();
    }

    public async Task SoftDeleteAll()
    {
        Todos.AttachRange();
        // todo: attach.. a whole range?
        await SaveChangesAsync();
    }

    //todo: change to a bool, use save change async > 0 
    public async Task<Todo> DeleteById(Guid id)
    {
        var todo = await Todos
            .FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            return null;
        }

        Todos.Attach(todo);
        Todos.Remove(todo);
        await SaveChangesAsync();
        return todo;
    }

    // todo: return bool if count == what we had in todos, we are good.
    public async Task DeleteAll()
    {
        Todos.RemoveRange(Todos);
        await SaveChangesAsync();
    }
}