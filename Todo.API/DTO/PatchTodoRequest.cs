using System.ComponentModel.DataAnnotations;
using TodoAPI.Controllers;

namespace TodoAPI.DTO;

public class PatchTodoRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [StringLength(1024)]
    public string? Name { get; set; }
    
    [StringLength(1024)]
    public string? Description { get; set; }
    
    [Range(0, 4)]
    public Priority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    
    public bool? IsCompleted { get; set; }
}