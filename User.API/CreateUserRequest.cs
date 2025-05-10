using System.ComponentModel.DataAnnotations;

namespace UserAPI;

public class CreateUserResponse
{
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string AccessToken { get; set; }
}

public class CreateUserRequest
{
    [Required]
    public string Name { get; set; }
    
    // Add email validation
    [Required]
    public string EmailAddress { get; set; }
    
    // Add password validation
        // length, special chars
    [Required]
    public string Password { get; set; }
}