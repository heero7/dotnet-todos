namespace UserAPI;
public interface IUserRepository
{
    Task<User?> Create(CreateUser createUser);
}

public class UserRepository(IUserPersistence userPersistence, ILogger<IUserRepository> logger) : IUserRepository
{
    private readonly ILogger _logger = logger;
    
    public Task<User?> Create(CreateUser createUser)
    {
        return userPersistence.Create(createUser);
    }
}