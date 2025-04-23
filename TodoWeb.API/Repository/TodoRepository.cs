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


public class CreateTodo
{
    public string Name { get; set; }
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

public interface ITodoRepository
{
    public Task Create(CreateTodo createTodo);
    public Task<List<Todo>> GetAll();
    public Task<Todo> GetById(Guid id);
    public Task Update(UpdateTodo updateTodo);
    public Task DeleteById(Guid id);
    public Task DeleteAll();
}
public class TodoRepository(ITodoPersistence todoPersistence) : ITodoRepository
{
    public Task Create(CreateTodo createTodo)
    {
        return todoPersistence.Create(createTodo);
    }

    public Task<List<Todo>> GetAll() => todoPersistence.GetAll();

    public async Task<Todo> GetById(Guid id) => await todoPersistence.GetById(id);

    public Task Update(UpdateTodo updateTodo)
    {
        return todoPersistence.Update(updateTodo);
    }

    public Task DeleteById(Guid id)
    {
        return todoPersistence.DeleteById(id);
    }

    public async Task DeleteAll() => await todoPersistence.DeleteAll();
}