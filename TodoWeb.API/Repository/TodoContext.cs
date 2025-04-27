using Microsoft.EntityFrameworkCore;
using TodoWeb.API.Controllers;
using TodoWeb.API.Models;

namespace TodoWeb.API.Repository;

public enum DeleteOperationStatus
{
    Success,
    EntityNotFound,
    SaveFailed
}

public interface ITodoPersistence
{
    public Task<Todo> Create(CreateTodo createTodo);
    public Task<List<Todo>> GetAll();
    public Task<Todo> GetById(Guid id);
    public Task<Todo> Update(UpdateTodo updateTodo);
    public Task<DeleteOperationStatus> SoftDeleteById(Guid id);
    public Task<DeleteOperationStatus> SoftDeleteAll();
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
        return Todos
            .Where(todo => todo.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Todo> GetById(Guid id)
    {
        var todo = await Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            _logger.LogError("Getting Todo with ID: {id} was null." +
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
            _logger.LogError("Getting Todo with ID: {id} was null. The todo will be null", updateTodo.Id);
            return null;
        }
        

        Todos.Attach(existingTodo);

        if (string.Equals(existingTodo.Name, updateTodo.Name))
        {
            _logger.LogWarning("A todo with this name already exists for this account.");
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

    public async Task<DeleteOperationStatus> SoftDeleteById(Guid id)
    {
        var todo = await Todos
            .FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            _logger.LogError("Getting Todo with ID: {id} was null.", id);
            return DeleteOperationStatus.EntityNotFound;
        }

        Todos.Attach(todo);
        todo.DeletedAt = DateTime.Now;
        var changes = await SaveChangesAsync();
        if (changes != 1)
        {
            _logger.LogError("Expected Single SoftDelete was off. Expected=1, Actual={act}", changes);
            return DeleteOperationStatus.SaveFailed;
        }

        return DeleteOperationStatus.Success;
    }

    public async Task<DeleteOperationStatus> SoftDeleteAll()
    {
        Todos.AttachRange();
        var allTodos = await Todos.ToListAsync();
        var expectedOperations = allTodos.Count;
        foreach (var todo in allTodos)
        {
            todo.DeletedAt = DateTime.Now;
        }
        
        var actualOperations = await SaveChangesAsync();
        if (actualOperations != expectedOperations)
        {
            _logger.LogError("Expected soft deletes were off. Expected={ex}, Actual={act}", 
                expectedOperations,
                actualOperations);
            return DeleteOperationStatus.SaveFailed;
        }

        return DeleteOperationStatus.Success;
    }

    //todo: change to a bool, use save change async > 0 
    public async Task<Todo> DeleteById(Guid id)
    {
        var todo = await Todos
            .FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            _logger.LogError("Getting Todo with ID: {id} was null.", id);
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