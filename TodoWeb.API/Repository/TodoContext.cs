using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TodoWeb.API.Controllers;

namespace TodoWeb.API.Repository;

public interface ITodoPersistence
{
    public Task Create(CreateTodo createTodo);
    public Task<List<Todo>> GetAll();
    public Task<Todo> GetById(Guid id);
    public Task Update(UpdateTodo updateTodo);
    public Task DeleteById(Guid id);
    public Task DeleteAll();
}

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options), ITodoPersistence
{
    public DbSet<Todo> Todos { get; set; }
    
    public async Task Create(CreateTodo createTodo)
    {
        Debug.Assert(createTodo.Name != null);
        
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Name = createTodo.Name,
            CreatedDate = DateTime.Now,
            Status = TodoStatus.Unclaimed,
            DueDate = createTodo.DueDate
        };
        await Todos.AddAsync(todo);
        await SaveChangesAsync();
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