using Microsoft.AspNetCore.Mvc;

namespace TodoWeb.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(ITodoService todoService) : ControllerBase
{

    [HttpPost]
    public ActionResult Create([FromBody] TodoRequest createTodoRequest)
    {
        if (string.IsNullOrEmpty(createTodoRequest.Name))
        {
            return BadRequest("Sorry, you can't create a todo without a name.");
        }
        
        todoService.AddTodo(createTodoRequest);
        
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Guid was empty, that todo doesn't exist.");
        }

        var todo = await todoService.GetById(id);
        var todoResponse = new TodoResponse
        {
            Id = todo.Id,
        };
        return Ok(todoResponse);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> GetAll()
    {
        var todos = await todoService.GetAll();
        var enumerable = todos as TodoResponse[] ?? todos.ToArray();
        if (enumerable.Length != 0)
        {
            var todoResponses = new TodoResponse[enumerable.Length];
            for (var i = 0; i < enumerable.Length; i++)
            {
                todoResponses[i] = new TodoResponse
                {
                    Id = enumerable[i].Id,
                    Name = enumerable[i].Name,
                    DueDate = enumerable[i].DueDate,
                    Status = enumerable[i].Status
                };
            }

            return Ok(todoResponses);
        }

        Console.WriteLine("Nothing was loaded");
        return Ok(Enumerable.Empty<TodoResponse>());
    }
}