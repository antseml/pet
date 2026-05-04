using Microsoft.EntityFrameworkCore;
using Lesson2.Models;
using System.Data.Common;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users{get; set;}
    public DbSet<Post> Posts{get; set;}
    public DbSet<Comment> Comments{get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(u => u.Email)
                .IsRequired();
        });
        modelBuilder.Entity<Post>(entity =>
        {
            entity.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(p => p.Content)
                .HasMaxLength(2000);
        });
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(c => c.Text)
                .HasMaxLength(1000);
            entity.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}