using TodoWeb.API.Controllers;

namespace TodoWeb.API.Models;

public class CreateTodo
{
    public string Name { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Description { get; set; }
    public Priority? Priority { get; set; }
}