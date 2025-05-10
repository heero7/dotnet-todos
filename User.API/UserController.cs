using Microsoft.AspNetCore.Mvc;

namespace UserAPI;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
   private readonly ILogger _logger = logger;

   [HttpPost("signup")]
   public async Task<ActionResult> SignUp([FromBody] CreateUserRequest createUserRequest)
   {
      return Ok();
   }

   [HttpPost("signin")]
   public async Task<ActionResult> SignIn([FromBody] UserSignInRequest userSignInRequest)
   {
      return Ok();
   }
}

public class UserSignInRequest
{
}