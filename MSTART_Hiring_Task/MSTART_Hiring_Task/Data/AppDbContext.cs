using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSTART_Hiring_Task.Models;

namespace MSTART_Hiring_Task.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<AppUser>().Property(u => u.ImageProfile)
                .HasDefaultValue("~/img/user.png");
            modelBuilder.Entity<Account>()
                .HasIndex(u => u.Account_Number)
                .IsUnique();
            modelBuilder.Entity<Account>()
               .HasOne(u => u.User)
               .WithMany(u => u.Accounts)
               .HasForeignKey(a => a.User_ID);
            modelBuilder.Entity<Account>()
              .HasOne(u => u.User)
              .WithMany(u => u.Accounts)
              .HasForeignKey(u => u.User_ID)
              .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Transactions)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.User_Id)
                .OnDelete(DeleteBehavior.NoAction); // Do not cascade delete.
            modelBuilder.Entity<Account>()
                .HasMany(u => u.Transactions)
                .WithOne(u => u.Account)
                .HasForeignKey(u => u.Account_Id)
                .OnDelete(DeleteBehavior.NoAction); // Do not cascade delete.
        }
    }
}
