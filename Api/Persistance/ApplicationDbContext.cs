using Api.Features.Accounts.Withdraw;
using Api.Features.Users;
using Api.Features.Users.Auth.RefreshToken;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Persistance
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):IdentityDbContext<ApplicationUser,ApplicationRole,Guid>(options)
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<WithdrawalRequest> Withdrawals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new RefreshTokenConfiguration());

            base.OnModelCreating(builder);


            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            });
        }
    }
}
