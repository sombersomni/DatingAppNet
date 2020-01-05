using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;

namespace DatingApp.API.Data {
    public class DatingDbContext : DbContext {

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DatingDbContext(DbContextOptions<DatingDbContext> options) : base(options) {}

    }
}