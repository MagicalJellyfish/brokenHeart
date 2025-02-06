using brokenHeart.Authentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Authentication.DB
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
