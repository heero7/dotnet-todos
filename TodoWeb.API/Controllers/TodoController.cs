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
    public ActionResult<TodoResponse> Create([FromBody] TodoRequest createTodoRequest)
    {
        if (string.IsNullOrEmpty(createTodoRequest.Name))
        {
            _logger.LogWarning("Validation error, Name was null/empty.");
            return BadRequest("Validation Error creating todo.");
        }
        
        var todo = todoService.AddTodo(createTodoRequest);
        
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

        if (enumerable.Length != 0)
        {
            return Ok(enumerable);
        }
        
        _logger.LogWarning("No todos to load.");
        return Ok(Enumerable.Empty<TodoResponse>());
    }

    [HttpPatch]
    public async Task<ActionResult<TodoResponse>> Patch([FromBody] PatchTodoRequest patchTodoRequest)
    {
        if (patchTodoRequest.Id == Guid.Empty)
        {
            _logger.LogWarning("Cannot have an empty id patching todo.");
            return BadRequest("Validation Error patching todo.");
        }

        var updatedTodo = await todoService.Update(patchTodoRequest);
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
}