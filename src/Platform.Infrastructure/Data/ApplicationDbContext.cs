using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Platform.Domain.Common;
using Platform.Domain.Entities;
using Platform.Infrastructure.Identity;

namespace Platform.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreUser> StoreUsers => Set<StoreUser>();
    public DbSet<StoreSettings> StoreSettings => Set<StoreSettings>();
    public DbSet<ThemeSettings> ThemeSettings => Set<ThemeSettings>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductFieldDefinition> ProductFieldDefinitions => Set<ProductFieldDefinition>();
    public DbSet<ProductCustomFieldValue> ProductCustomFieldValues => Set<ProductCustomFieldValue>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageLayout> PageLayouts => Set<PageLayout>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigureStores(builder);
        ConfigureCatalog(builder);
        ConfigurePages(builder);
        ConfigureOrders(builder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        StampEntities();
        return base.SaveChanges();
    }

    private void StampEntities()
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    private static void ConfigureStores(ModelBuilder builder)
    {
        builder.Entity<Store>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Domain).HasMaxLength(255);
            entity.Property(x => x.ThemeName).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.HasIndex(x => x.Domain).IsUnique().HasFilter("[Domain] IS NOT NULL");
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.DisplayName).HasMaxLength(180);
            entity.HasMany(x => x.StoreUsers)
                .WithOne()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StoreUser>(entity =>
        {
            entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            entity.HasIndex(x => new { x.StoreId, x.UserId }).IsUnique();
            entity.HasOne(x => x.Store)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StoreSettings>(entity =>
        {
            entity.Property(x => x.Currency).HasMaxLength(8).IsRequired();
            entity.Property(x => x.Culture).HasMaxLength(16).IsRequired();
            entity.Property(x => x.ContactEmail).HasMaxLength(255);
            entity.Property(x => x.SettingsJson).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => x.StoreId).IsUnique();
            entity.HasOne(x => x.Store)
                .WithOne(x => x.Settings)
                .HasForeignKey<StoreSettings>(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ThemeSettings>(entity =>
        {
            entity.Property(x => x.ThemeName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.PrimaryColor).HasMaxLength(24).IsRequired();
            entity.Property(x => x.AccentColor).HasMaxLength(24).IsRequired();
            entity.Property(x => x.FontFamily).HasMaxLength(160).IsRequired();
            entity.Property(x => x.CustomCss).HasColumnType("nvarchar(max)");
            entity.Property(x => x.SettingsJson).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => x.StoreId).IsUnique();
            entity.HasOne(x => x.Store)
                .WithOne(x => x.ThemeSettings)
                .HasForeignKey<ThemeSettings>(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureCatalog(ModelBuilder builder)
    {
        builder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(220).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Sku).HasMaxLength(80);
            entity.Property(x => x.Description).HasColumnType("nvarchar(max)");
            entity.Property(x => x.BasePrice).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.StoreId, x.Slug }).IsUnique();
            entity.HasIndex(x => new { x.StoreId, x.Status });
            entity.HasOne(x => x.Store)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductFieldDefinition>(entity =>
        {
            entity.Property(x => x.Key).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Label).HasMaxLength(160).IsRequired();
            entity.Property(x => x.OptionsJson).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => new { x.StoreId, x.Key }).IsUnique();
            entity.HasOne(x => x.Store)
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductCustomFieldValue>(entity =>
        {
            entity.Property(x => x.ValueJson).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => new { x.ProductId, x.ProductFieldDefinitionId }).IsUnique();
            entity.HasOne(x => x.Product)
                .WithMany(x => x.CustomFieldValues)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.FieldDefinition)
                .WithMany()
                .HasForeignKey(x => x.ProductFieldDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.Property(x => x.Url).HasMaxLength(1200).IsRequired();
            entity.Property(x => x.AltText).HasMaxLength(255);
            entity.HasOne(x => x.Product)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => new { x.StoreId, x.Slug }).IsUnique();
            entity.HasOne(x => x.Store)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.ParentCategory)
                .WithMany()
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(x => new { x.ProductId, x.CategoryId });
            entity.HasIndex(x => new { x.StoreId, x.CategoryId });
            entity.HasOne(x => x.Product)
                .WithMany(x => x.ProductCategories)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Category)
                .WithMany(x => x.ProductCategories)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigurePages(ModelBuilder builder)
    {
        builder.Entity<Page>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(220).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(160).IsRequired();
            entity.HasIndex(x => new { x.StoreId, x.Slug }).IsUnique();
            entity.HasOne(x => x.Store)
                .WithMany(x => x.Pages)
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PageLayout>(entity =>
        {
            entity.Property(x => x.LayoutJson).HasColumnType("nvarchar(max)").IsRequired();
            entity.HasIndex(x => new { x.PageId, x.Version }).IsUnique();
            entity.HasOne(x => x.Page)
                .WithMany(x => x.Layouts)
                .HasForeignKey(x => x.PageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureOrders(ModelBuilder builder)
    {
        builder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.Email).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(40);
            entity.HasIndex(x => new { x.StoreId, x.Email });
            entity.HasOne(x => x.Store)
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Order>(entity =>
        {
            entity.Property(x => x.OrderNumber).HasMaxLength(40).IsRequired();
            entity.Property(x => x.CustomerEmail).HasMaxLength(255).IsRequired();
            entity.Property(x => x.CustomerName).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Subtotal).HasPrecision(18, 2);
            entity.Property(x => x.Tax).HasPrecision(18, 2);
            entity.Property(x => x.Shipping).HasPrecision(18, 2);
            entity.Property(x => x.Total).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.StoreId, x.OrderNumber }).IsUnique();
            entity.HasOne(x => x.Store)
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(x => x.ProductName).HasMaxLength(220).IsRequired();
            entity.Property(x => x.Sku).HasMaxLength(80);
            entity.Property(x => x.UnitPrice).HasPrecision(18, 2);
            entity.Property(x => x.LineTotal).HasPrecision(18, 2);
            entity.Property(x => x.CustomSnapshotJson).HasColumnType("nvarchar(max)");
            entity.HasOne(x => x.Order)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
