using TodoAPI.Controllers;

namespace TodoAPI.Models;

public class UpdateTodo
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority? Priority { get; set; }
    public bool? IsCompleted { get; set; }
}