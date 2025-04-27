using Todo.API.Models;

namespace Todo.API.Repository;

public interface ITodoRepository
{
    public Task<Models.Todo> Create(CreateTodo createTodo);
    public Task<List<Models.Todo>> GetAll();
    public Task<Models.Todo> GetById(Guid id);
    public Task<Models.Todo> Update(UpdateTodo updateTodo);
    public Task DeleteById(Guid id);
    public Task DeleteAll();
    public Task<DeleteOperationStatus> SoftDeleteById(Guid id);
    public Task<DeleteOperationStatus> SoftDeleteAll();
}
public class TodoRepository(ITodoPersistence todoPersistence) : ITodoRepository
{
    
    public Task<Models.Todo> Create(CreateTodo createTodo)
    {
        return todoPersistence.Create(createTodo);
    }

    public Task<List<Models.Todo>> GetAll() => todoPersistence.GetAll();

    public async Task<Models.Todo> GetById(Guid id) => await todoPersistence.GetById(id);

    public Task<Models.Todo> Update(UpdateTodo updateTodo)
    {
        return todoPersistence.Update(updateTodo);
    }

    public Task DeleteById(Guid id)
    {
        return todoPersistence.DeleteById(id);
    }

    public async Task DeleteAll() => await todoPersistence.DeleteAll();
    public Task<DeleteOperationStatus> SoftDeleteById(Guid id)
    {
        return todoPersistence.SoftDeleteById(id);
    }

    public Task<DeleteOperationStatus> SoftDeleteAll()
    {
        return todoPersistence.SoftDeleteAll();
    }
}