using System.Security.Cryptography;
using System.Text;
using todo.Shared;
using Microsoft.EntityFrameworkCore;

namespace todo.Server.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<TodoItem> Todos { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Email);

        // modelBuilder.Entity<TodoItem>()
        //     .HasOne(t => t.User)
        //     .WithMany(u => u.Todos)
        //     .HasForeignKey(t => t.UserId);
    }
}