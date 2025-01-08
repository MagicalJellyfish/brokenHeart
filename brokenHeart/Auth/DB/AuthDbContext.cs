using brokenHeart.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Auth.DB
{
    public class AuthDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public string DbPath { get; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        public AuthDbContext() { }
    }
}
