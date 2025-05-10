using Microsoft.EntityFrameworkCore;

namespace UserAPI;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public bool IsActive { get; set; }
}

public interface IUserPersistence
{
    Task<User?> Create(CreateUser createUser);
    Task<User?> GetByEmailAddress();
}

public class CreateUser
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserDbContext(DbContextOptions<UserDbContext> options,ILogger<UserDbContext> logger) 
    : DbContext(options), IUserPersistence
{
    private readonly ILogger _logger = logger;
    
    public DbSet<User> Users { get; set; }
    
    public async Task<User?> Create(CreateUser createUser)
    {
        var existingUser = await Users
            .FirstOrDefaultAsync(user => string.Equals(createUser.Email, user.Email));
        if (existingUser != null)
        {
            _logger.LogError("A user already exists with this email address => {em}.", createUser.Email);
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = createUser.Name,
            Email = createUser.Email,
            IsActive = true,
            LastLoginDate = DateTime.MinValue,
            ModifiedDate = DateTime.Now,
            CreatedDate = DateTime.Now,
            // todo: work todo with passwords
            PasswordHash = createUser.Password
        };
        await Users.AddAsync(user);
        await SaveChangesAsync();
        return user;
    }

    public Task<User?> GetByEmailAddress()
    {
        throw new NotImplementedException();
    }
}