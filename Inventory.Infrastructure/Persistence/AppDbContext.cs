using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Shortcode).IsRequired().HasMaxLength(50);
            b.HasIndex(x => x.Shortcode).IsUnique();
            b.HasOne(x => x.ParentCategory)
             .WithMany(x => x.Children)
             .HasForeignKey(x => x.ParentCategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Supplier>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Email).IsRequired().HasMaxLength(320);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(10);
            b.Property(x => x.Country).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Description).IsRequired().HasMaxLength(500);
            b.Property(x => x.AcquisitionCostSupplierCurrency).HasColumnType("numeric(18,2)");
            b.Property(x => x.AcquisitionCostUSD).HasColumnType("numeric(18,2)");
            b.HasIndex(x => x.Status);
        });
    }
}
