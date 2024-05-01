using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Auth.DB
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public string DbPath { get; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        public AuthDbContext() { }
    }
}
