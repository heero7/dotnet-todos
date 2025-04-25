using System.ComponentModel.DataAnnotations;
using TodoWeb.API.Controllers;

namespace TodoWeb.API.Dto;

public class PatchTodoRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [StringLength(1024)]
    public string? Name { get; set; }
    
    [Range(0, 4)]
    public TodoStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
}