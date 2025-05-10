namespace UserAPI;

public interface IUserService
{
    Task<CreateUserResponse?> Create(CreateUserRequest createUserRequest);
}

public class UserService(IUserRepository userRepository, ILogger<IUserService> logger) : IUserService
{
    private readonly ILogger _logger = logger;
    
    public async Task<CreateUserResponse?> Create(CreateUserRequest createUserRequest)
    {
        var createdUser = await userRepository.Create(new CreateUser
        {
            Name = createUserRequest.Name,
            Email = createUserRequest.EmailAddress,
            Password = createUserRequest.Password
        });

        if (createdUser == null)
        {
            // this is because we know why this can be null and the global exception
            // will handle other errors.
            _logger.LogError("A user already exists with that email address.");
            return null;
        }

        return new CreateUserResponse();
    }
}