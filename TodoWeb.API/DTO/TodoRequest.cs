using System.ComponentModel.DataAnnotations;
using TodoWeb.API.Controllers;

namespace TodoWeb.API.Dto;

public class TodoRequest
{
    
    [Required]
    [StringLength(1024)]
    public string Name { get; set; }
    public TodoStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
}