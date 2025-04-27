using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoWeb.API.Dto;

namespace TodoWeb.API.Controllers;

public class NotInPast : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.Now;
        }

        return true;
    }
}

[ApiController]
[Route("api/[controller]")]
public class TodoController(ITodoService todoService, ILogger<TodoController> logger) : ControllerBase
{
    private readonly ILogger _logger = logger;
    
    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] TodoRequest createTodoRequest)
    {
        var todo = await todoService.AddTodo(createTodoRequest);
        if (todo == null)
        {
            return BadRequest();
        }
        
        return Ok(todo);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoResponse>> GetById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Guid was empty, that todo doesn't exist.");
        }

        var todoResponse = await todoService.GetById(id);
        if (todoResponse == null)
        {
            return NotFound();
        }
        
        return Ok(todoResponse);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> GetAll()
    {
        var todos = await todoService.GetAll();
        var enumerable = todos as TodoResponse[] ?? todos.ToArray();

        return Ok(enumerable.Length != 0 ? enumerable : Enumerable.Empty<TodoResponse>());
    }

    [HttpPatch]
    public async Task<ActionResult<TodoResponse>> Patch([FromBody] PatchTodoRequest patchTodoRequest)
    {
        var updatedTodo = await todoService.Update(patchTodoRequest);
        if (updatedTodo == null)
        {
            return BadRequest();
        }
        return Ok(updatedTodo);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteById(Guid id)
    {
        if (Guid.Empty == id)
        {
            _logger.LogWarning("Id passed in to DeleteById was empty => {id}.", id);
            return BadRequest();
        }

        await todoService.DeleteById(id);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete()
    {
        await todoService.DeleteAll();
        return NoContent();
    }

    [HttpDelete("softdelete/{id:guid}")]
    public async Task<ActionResult> SoftDeleteById(Guid id)
    {
        if (Guid.Empty == id)
        {
            _logger.LogWarning("Id passed in to SoftDeleteById was empty");
            return BadRequest("Guid was empty.");
        }
        
        var result = await todoService.SoftDeleteById(id);
        return result switch
        {
            DeleteStatus.Success => Ok(),
            DeleteStatus.Failure => Problem(statusCode: 500, title: "Internal Server Error", detail: "We had trouble processing your request"),
            DeleteStatus.BadUserInput => BadRequest("A todo with this id does not exist"),
            _ => Problem(statusCode: 500, title: "Internal Server Error", detail: "We had trouble processing your request")
        };
    }
    
    [HttpDelete("softdelete/")]
    public async Task<ActionResult> SoftDeleteAll()
    {
        var result = await todoService.SoftDeleteAll();
        return result switch
        {
            DeleteStatus.Success => Ok(),
            DeleteStatus.Failure => Problem(statusCode: 500, title: "Internal Server Error", detail: "We had trouble processing your request"),
            DeleteStatus.BadUserInput => BadRequest("A todo with this id does not exist"),
            _ => Problem(statusCode: 500, title: "Internal Server Error", detail: "We had trouble processing your request")
        };
    }
}