namespace HydraSim.Domain.Auth
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<RegisterResult> RegisterAsync(string email, string password);
    }
}
