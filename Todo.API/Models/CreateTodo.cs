using TodoAPI.Controllers;

namespace TodoAPI.Models;

public class CreateTodo
{
    public string Name { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Description { get; set; }
    public Priority? Priority { get; set; }
}