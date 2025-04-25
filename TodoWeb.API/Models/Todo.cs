using TodoWeb.API.Controllers;

namespace TodoWeb.API.Models;

public class Todo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TodoStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}