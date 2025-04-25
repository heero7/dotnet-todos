using TodoWeb.API.Controllers;

namespace TodoWeb.API.Dto;

public class TodoResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TodoStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
}