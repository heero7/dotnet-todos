using System.ComponentModel.DataAnnotations;
using TodoWeb.API.Controllers;

namespace TodoWeb.API.Dto;

public class TodoRequest
{
    
    [Required]
    [StringLength(512)]
    public string Name { get; set; }
    
    [StringLength(1024)]
    public string? Description { get; set; }
    
    [Range(0, 4)]
    public Priority? Priority { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }
}