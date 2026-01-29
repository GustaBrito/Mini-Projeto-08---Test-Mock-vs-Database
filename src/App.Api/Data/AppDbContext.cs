using App.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.Description)
                .HasMaxLength(500);
            builder.Property(p => p.Price)
                .HasPrecision(10, 2);
            builder.Property(p => p.CreatedAtUtc)
                .IsRequired();
        });
    }
}
