using Microsoft.EntityFrameworkCore;
using BusinessTracker.Data.Models;

namespace BusinessTracker.Data;

public partial class BusinessTrackerContext : DbContext
{
    public BusinessTrackerContext()
    {
    }

    public BusinessTrackerContext(DbContextOptions<BusinessTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<LinksUserOrganization> LinksUserOrganizations { get; set; }

    public virtual DbSet<Nomenclature> Nomenclatures { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(
            "User ID=admin;Password=123456;Host=localhost;Port=5433;Database=business_tracker;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Categories_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Owner).WithMany(p => p.Categories)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Categories_Organizations");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Employees_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Owner).WithMany(p => p.Employees)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Employees_Organizations");
        });

        modelBuilder.Entity<LinksUserOrganization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LinksUserOrganizations_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Organization).WithMany(p => p.LinksUserOrganizations)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_LinksUserOrganizations_Organizations");

            entity.HasOne(d => d.User).WithMany(p => p.LinksUserOrganizations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_LinksUserOrganizations_Users");
        });

        modelBuilder.Entity<Nomenclature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Nomenclatures_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Nomenclatures)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Nomenclatures_Categories");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Organizations_pkey");

            entity.HasIndex(e => e.Inn, "OrganizationInnIx").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Inn).HasMaxLength(10);
            entity.Property(e => e.LoadOptions).HasColumnType("jsonb");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Transactions_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Amount).HasPrecision(15, 2);
            entity.Property(e => e.Discount).HasPrecision(15, 2);
            entity.Property(e => e.Quantity).HasPrecision(15, 2);
            entity.Property(e => e.PaymentType).HasDefaultValue(1);

            entity.HasOne(d => d.Employee).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Transactions_Employees");

            entity.HasOne(d => d.Nomenclature).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.NomenclatureId)
                .HasConstraintName("FK_Transactions_Nomenclatures");

            entity.HasOne(d => d.Owner).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Transactions_Organizations");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}