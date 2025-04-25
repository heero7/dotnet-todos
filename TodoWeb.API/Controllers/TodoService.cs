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
            Priority = createTodoRequest.Priority,
            Description = createTodoRequest.Description
        });
        
        return new TodoResponse
        {
            Id = todo.Id,
            Name = todo.Name,
            DueDate = todo.DueDate,
            Priority = todo.Priority
        };
    }

    public async Task<IEnumerable<TodoResponse>> GetAll()
    {
        var todos = await todoRepository.GetAll();
        var todoResponses = todos.Select(todo => new TodoResponse
            {
                Id = todo.Id, Name = todo.Name, Priority = todo.Priority, DueDate = todo.DueDate,
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
             Priority = todoById.Priority
         };
    }

    public async Task<TodoResponse> Update(PatchTodoRequest patchTodoRequest)
    {
        var updatedTodo = await todoRepository.Update(new UpdateTodo
        {
            Id = patchTodoRequest.Id,
            Name = patchTodoRequest.Name,
            DueDate = patchTodoRequest.DueDate,
            Priority = patchTodoRequest.Priority,
            IsCompleted = patchTodoRequest.IsCompleted
        });

        return new TodoResponse
        {
            Id = updatedTodo.Id,
            Name = updatedTodo.Name,
            DueDate = updatedTodo.DueDate,
            Priority = updatedTodo.Priority
        };
    }

    public async Task DeleteById(Guid id) => await todoRepository.DeleteById(id);

    public async Task DeleteAll() => await todoRepository.DeleteAll();
}