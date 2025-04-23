using TodoWeb.API.Repository;

namespace TodoWeb.API.Controllers;

public interface ITodoService
{
    public void AddTodo(TodoRequest createTodoRequest);
    Task<IEnumerable<TodoResponse>> GetAll();
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

    public async Task<IEnumerable<TodoResponse>> GetAll()
    {
        var todos = await todoRepository.GetAll();
        var todoResponses = todos.Select(todo => new TodoResponse
            {
                Id = todo.Id, Name = todo.Name, Status = todo.Status, DueDate = todo.DueDate,
            })
            .ToList();
        return todoResponses;
    }

    public Task<TodoResponse> GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}