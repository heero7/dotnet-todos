namespace TodoWeb.API.Controllers;

public class TodoRequest
{
    public string Name { get; set; }
    public TodoStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
}

public class TodoResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TodoStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
}

public enum TodoStatus
{
    Unclaimed,
    InProgress,
    Completed,
    Overdue
}