using TodoWeb.API.Controllers;

namespace TodoWeb.API.Models;

public class UpdateTodo
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoStatus? Status { get; set; }
}