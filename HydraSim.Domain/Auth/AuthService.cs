namespace HydraSim.Domain.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly PasswordHasher _hasher;

        public AuthService(IUserRepository users, PasswordHasher hasher)
        {
            _users = users;
            _hasher = hasher;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _users.GetByEmailAsync(email);
            if (user == null) return null;
            if (!_hasher.Verify(password, user.PasswordHash)) return null;
            return user;
        }

        public async Task<RegisterResult> RegisterAsync(string email, string password)
        {
            var existing = await _users.GetByEmailAsync(email);
            if (existing != null)
            {
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "This e-mail is already in use."
                };
            }

            var user = new User
            {
                Email = email,
                PasswordHash = _hasher.Hash(password)
            };
            await _users.AddAsync(user);

            return new RegisterResult
            {
                Success = true,
                User = user
            };
        }
    }
}
