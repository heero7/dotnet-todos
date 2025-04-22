using TodoWeb.API.Repository;

namespace TodoWeb.API.Controllers;

public interface ITodoService
{
    public void AddTodo(TodoRequest createTodoRequest);
    IEnumerable<TodoResponse> GetAll();
    Task<TodoResponse> GetById(Guid id);
}

public class TodoService(ITodoRepository todoRepository) : ITodoService
{
    public void AddTodo(TodoRequest createTodoRequest)
    {
        todoRepository.Create(new CreateTodo
        {
            Name = createTodoRequest.Name,
            DueDate = createTodoRequest.DueDate,
            Status = createTodoRequest.Status
        });
    }

    public IEnumerable<TodoResponse> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<TodoResponse> GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}