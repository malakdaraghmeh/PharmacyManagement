using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Drug> Drugs { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<CreditRecord> CreditRecords { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<DrugSupplier> DrugSuppliers { get; set; }
    public DbSet<Batch> Batches { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<StockAdjustment> StockAdjustments { get; set; }
    public DbSet<StockReceive> StockReceives { get; set; }
    public DbSet<StockReceiveItem> StockReceiveItems { get; set; }
    public DbSet<ExpiryAlert> ExpiryAlerts { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PharmacyName).HasMaxLength(200);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Drug configuration (catalog)
        modelBuilder.Entity<Drug>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Barcode);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Barcode).HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Drugs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Drugs)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Manufacturer)
                .WithMany(m => m.Drugs)
                .HasForeignKey(e => e.ManufacturerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Unit configuration
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Manufacturer configuration
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Supplier configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // DrugSupplier (many-to-many join)
        modelBuilder.Entity<DrugSupplier>(entity =>
        {
            entity.HasKey(e => new { e.DrugId, e.SupplierId });
            entity.HasOne(e => e.Drug)
                .WithMany(d => d.DrugSuppliers)
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.DrugSuppliers)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Batch configuration
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.SellingPrice).HasPrecision(18, 2);

            entity.HasOne(e => e.Drug)
                .WithMany(d => d.Batches)
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Batches)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Sale configuration
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.NetAmount).HasPrecision(18, 2);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Sales)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // SaleItem configuration
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

            entity.HasOne(e => e.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(e => e.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Drug)
                .WithMany(d => d.SaleItems)
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // CreditRecord configuration
        modelBuilder.Entity<CreditRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
            entity.Property(e => e.RemainingAmount).HasPrecision(18, 2);

            entity.Property(e => e.Type).HasConversion<int>(); // هذا يحول TransactionType إلى int في DB

            entity.Property(e => e.Status).HasConversion<int>();


            entity.HasOne(e => e.User)
                .WithMany(u => u.CreditRecords)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // StockMovement configuration
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalValue).HasPrecision(18, 2);
            entity.Property(e => e.Type).HasConversion<int>();

            entity.HasOne(e => e.Drug).WithMany().HasForeignKey(e => e.DrugId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Batch).WithMany().HasForeignKey(e => e.BatchId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // StockAdjustment configuration
        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.Drug).WithMany().HasForeignKey(e => e.DrugId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Batch).WithMany().HasForeignKey(e => e.BatchId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // StockReceive configuration
        modelBuilder.Entity<StockReceive>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);

            entity.HasOne(e => e.Supplier).WithMany().HasForeignKey(e => e.SupplierId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // StockReceiveItem configuration
        modelBuilder.Entity<StockReceiveItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.SellingPrice).HasPrecision(18, 2);
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);

            entity.HasOne(e => e.StockReceive)
                .WithMany(r => r.Items)
                .HasForeignKey(e => e.StockReceiveId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Drug).WithMany().HasForeignKey(e => e.DrugId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ExpiryAlert configuration
        modelBuilder.Entity<ExpiryAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EstimatedLossValue).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Severity).HasConversion<int>();
            entity.Property(e => e.RecommendedAction).HasConversion<int>();

            entity.HasOne(e => e.Batch).WithMany().HasForeignKey(e => e.BatchId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Drug).WithMany().HasForeignKey(e => e.DrugId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // SystemSettings configuration
        modelBuilder.Entity<SystemSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TaxPercentage).HasPrecision(18, 2);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // SQL Server does not allow multiple cascade paths or cycles. Since this domain relies on
        // soft-deletes (IsDeleted query filters), hard cascade deletes never fire, so all foreign
        // keys are set to NoAction to keep the schema valid.
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }
}