using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatingDbContext _context;
        public AuthRepository(DatingDbContext context) {
            _context = context;
        }
        public async Task<User> Register(User user, string password) {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                // turns the password string into a byte array for the compute hash functions
                byte[] passwordBuffer = System.Text.Encoding.UTF8.GetBytes(password);
                passwordHash = hmac.ComputeHash(passwordBuffer);
            }
        }
        public async Task<User> Login(string username, string password) {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.UserName == username);
            if (user == null) {
                return null;
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                return null;
            }
            return user;
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                // turns the password string into a byte array for the compute hash functions
                byte[] passwordBuffer = System.Text.Encoding.UTF8.GetBytes(password);
                var computedHash = hmac.ComputeHash(passwordBuffer);
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }
        public async Task<bool> UserExists(string username) {
            return await _context.Users.AnyAsync(user => user.UserName == username); 
        }
    }
}