using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace User.API;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
   private readonly ILogger _logger = logger;

   [HttpPost("signup")]
   public async Task<ActionResult> Post([FromBody] CreateUserRequest createUserRequest)
   {
      return Ok();
   }
   
}

public class CreateUserRequest
{
}

public class UserService : IUserService
{
    
}

public interface IUserService
{
}

public class UserRepository : IUserRepository
{
    
}

public interface IUserRepository
{
}

public class UserDbContext : DbContext, IUserRepository
{
    
}