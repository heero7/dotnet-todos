using Microsoft.EntityFrameworkCore;
using TodoWeb.API.Controllers;

namespace TodoWeb.API.Repository;

public class Todo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TodoStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public interface ITodoPersistence
{
    public Task Create(CreateTodo createTodo);
    public IEnumerable<Todo> GetAll();
    public Task<Todo> GetById(Guid id);
    public Task Update(UpdateTodo updateTodo);
    public Task DeleteById(Guid id);
    public Task DeleteAll();
}

public class CreateTodo
{
    public string? Name { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoStatus? Status { get; set; }
}
public class UpdateTodo
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoStatus? TodoStatus { get; set; }
}

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options), ITodoPersistence
{
    public DbSet<Todo> Todos { get; set; }
    
    public async Task Create(CreateTodo createTodo)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.Now,
            Status = TodoStatus.Unclaimed,
            DueDate = createTodo.DueDate
        };
        await Todos.AddAsync(todo);
        await SaveChangesAsync();
    }

    public IEnumerable<Todo> GetAll()
    {
        return Todos;
    }

    public async Task<Todo> GetById(Guid id)
    {
        var todo = await Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            Console.WriteLine($"Getting Todo with ID: {id} was null." +
                              $"The todo will be null");
        }
        return todo;
    }

    public async Task Update(UpdateTodo updateTodo)
    {
        var todo = await Todos
            .FirstOrDefaultAsync(t => t.Id == updateTodo.Id);
        if (todo == null)
        {
            Console.WriteLine($"Getting Todo with ID: {updateTodo.Id} was null." +
                              $"The todo will be null");
            return;
        }

        Todos.Attach(todo);

        todo.Name = updateTodo?.Name ?? todo.Name;
        todo.Status = updateTodo?.TodoStatus ?? todo.Status;
        todo.DueDate = updateTodo?.DueDate;

        await SaveChangesAsync();
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

public interface ITodoRepository
{
    public Task Create(CreateTodo createTodo);
    public IEnumerable<Todo> GetAll();
    public Task<Todo> GetById(Guid id);
    public Task Update(UpdateTodo updateTodo);
    public Task DeleteById(Guid id);
    public Task DeleteAll();
}
public class TodoRepository(ITodoPersistence todoPersistence) : ITodoRepository
{
    private readonly ITodoPersistence _todoPersistence = todoPersistence;
    
    public Task Create(CreateTodo createTodo)
    {
        return _todoPersistence.Create(createTodo);
    }

    public IEnumerable<Todo> GetAll() => _todoPersistence.GetAll();

    public async Task<Todo> GetById(Guid id) => await _todoPersistence.GetById(id);

    public Task Update(UpdateTodo updateTodo)
    {
        return _todoPersistence.Update(updateTodo);
    }

    public Task DeleteById(Guid id)
    {
        return _todoPersistence.DeleteById(id);
    }

    public async Task DeleteAll() => await _todoPersistence.DeleteAll();
}