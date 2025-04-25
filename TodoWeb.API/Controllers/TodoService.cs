using TodoWeb.API.Dto;
using TodoWeb.API.Models;
using TodoWeb.API.Repository;

namespace TodoWeb.API.Controllers;

public interface ITodoService
{
    public Task<TodoResponse> AddTodo(TodoRequest createTodoRequest);
    Task<IEnumerable<TodoResponse>> GetAll();
    Task<TodoResponse> GetById(Guid id);
    Task<TodoResponse> Update(PatchTodoRequest patchTodoRequest);
    Task DeleteById(Guid id);
    Task DeleteAll();
}

public class TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger) : ITodoService
{
    private readonly ILogger _logger = logger;
    public async Task<TodoResponse> AddTodo(TodoRequest createTodoRequest)
    {
        var todo = await todoRepository.Create(new CreateTodo
        {
            Name = createTodoRequest.Name,
            DueDate = createTodoRequest.DueDate,
            Status = createTodoRequest.Status
        });
        
        return new TodoResponse
        {
            Id = todo.Id,
            Name = todo.Name,
            DueDate = todo.DueDate,
            Status = todo.Status
        };
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

    public async Task<TodoResponse> GetById(Guid id)
    {
         var todoById = await todoRepository.GetById(id);
         if (todoById == null)
         {
             _logger.LogWarning("Could not find todo with {id}", id);
             return null;
         }
         
         return new TodoResponse
         {
             Id = todoById.Id,
             Name = todoById.Name,
             DueDate = todoById.DueDate,
             Status = todoById.Status
         };
    }

    public async Task<TodoResponse> Update(PatchTodoRequest patchTodoRequest)
    {
        var updatedTodo = await todoRepository.Update(new UpdateTodo
        {
            Id = patchTodoRequest.Id,
            Name = patchTodoRequest.Name,
            DueDate = patchTodoRequest.DueDate,
            Status = patchTodoRequest.Status
        });

        return new TodoResponse
        {
            Id = updatedTodo.Id,
            Name = updatedTodo.Name,
            DueDate = updatedTodo.DueDate,
            Status = updatedTodo.Status
        };
    }

    public async Task DeleteById(Guid id) => await todoRepository.DeleteById(id);

    public async Task DeleteAll() => await todoRepository.DeleteAll();
}