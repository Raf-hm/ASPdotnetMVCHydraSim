using HydraSim.DAL.Data;
using HydraSim.Domain.Auth;
using Microsoft.EntityFrameworkCore;

namespace HydraSim.DAL.Repositories
{
    public class UserRepository
    {
        private readonly HydraSimDbContext _db;

        public UserRepository(HydraSimDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}
