using TodoWeb.API.Controllers;

namespace TodoWeb.API.Models;

public class CreateTodo
{
    public string Name { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoStatus? Status { get; set; }
}