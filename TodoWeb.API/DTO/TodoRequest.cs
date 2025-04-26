using System.ComponentModel.DataAnnotations;
using TodoWeb.API.Controllers;

namespace TodoWeb.API.Dto;

public class TodoRequest
{
    
    [Required(ErrorMessage = "Name is a required field for TodoRequest")]
    [StringLength(512)]
    public string Name { get; set; }
    
    [StringLength(1024)]
    public string? Description { get; set; }
    
    [Range(0, 4)]
    public Priority? Priority { get; set; }
    
    [DataType(DataType.Date)]
    [NotInPast(ErrorMessage = "Date cannot be in the past.")]
    public DateTime? DueDate { get; set; }
}