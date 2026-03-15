using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using UserManagementAPI.Models;

namespace UserManagementAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Study> Studies => Set<Study>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<SessionLog> SessionLogs => Set<SessionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Addresses)
            .WithOne()
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Studies)
            .WithOne()
            .HasForeignKey(s => s.UserId);
    }
}
