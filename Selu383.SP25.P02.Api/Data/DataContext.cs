using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Features.Users;
using Selu383.SP25.P02.Api.Features.Roles;
using Selu383.SP25.P02.Api.Features.UserRoles;
using Selu383.SP25.P02.Api.Features.Theaters;

namespace Selu383.SP25.P02.Api.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, 
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Theater> Theaters { get; set; }
        public DbSet <UserRole> UserRoles {  get; set; }
        public DbSet <User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User-Role Many-to-Many Relationship
            builder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.Roles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        }
    }
}
