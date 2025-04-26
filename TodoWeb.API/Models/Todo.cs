using TodoWeb.API.Controllers;

namespace TodoWeb.API.Models;

public class Todo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsCompleted { get; set; }
}