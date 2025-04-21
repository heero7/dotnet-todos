using Microsoft.AspNetCore.Mvc;
using TodoWeb.API.Repository;

namespace TodoWeb.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(ITodoRepository todoRepository) : ControllerBase
{
    [HttpPost]
    public ActionResult Create([FromBody] TodoRequest createTodoRequest)
    {
        if (string.IsNullOrEmpty(createTodoRequest.Name))
        {
            return BadRequest("Sorry, you can't create a todo without a name.");
        }
        
        todoRepository.Create(new CreateTodo
        {
            Name = createTodoRequest.Name,
            DueDate = createTodoRequest.DueDate,
            Status = createTodoRequest.Status
        });
        
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Guid was empty, that todo doesn't exist.");
        }

        var todo = await todoRepository.GetById(id);
        var todoResponse = new TodoResponse
        {
            Id = todo.Id,
        };
        return Ok(todoResponse);
    }

    [HttpGet]
    public ActionResult<IEnumerable<TodoResponse>> GetAll()
    {
        var loadedTodos = todoRepository.GetAll();
        var todos = loadedTodos as Todo[] ?? loadedTodos.ToArray();
        if (todos.Length != 0)
        {
            var todoResponses = new TodoResponse[todos.Length];
            for (var i = 0; i < todos.Length; i++)
            {
                todoResponses[i] = new TodoResponse
                {
                    Id = todos[i].Id,
                    Name = todos[i].Name,
                    DueDate = todos[i].DueDate,
                    Status = todos[i].Status
                };
            }

            return Ok(todoResponses);
        }

        Console.WriteLine("Sorry, we had trouble loading those items. Try again.");
        return Ok(new List<TodoResponse>
        {
            new() { Id = Guid.NewGuid(), Name = "Clean mirrors", DueDate = DateTime.Now.AddDays(1), Status = TodoStatus.Unclaimed },
            new() { Id = Guid.NewGuid(), Name = "Take Basil out", DueDate = DateTime.Now.AddHours(2), Status = TodoStatus.Overdue },
            new() { Id = Guid.NewGuid(), Name = "Practice Dotnet", DueDate = DateTime.Now.AddDays(3), Status = TodoStatus.InProgress },
        });
    }
}