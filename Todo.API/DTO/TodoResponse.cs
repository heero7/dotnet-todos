using Todo.API.Controllers;

namespace Todo.API.Dto;

public class TodoResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    // todo: add a modified date to just know when this was updated (on create time == modified date, mod = any update or delete)
    // public DateTime ModifiedDate { get; set; }
}