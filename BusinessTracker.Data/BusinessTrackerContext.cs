using BusinessTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessTracker.Data;

/// <summary>
///     Контекст базы данных PostgreSQL для приложения BusinessTracker.
/// </summary>
public partial class BusinessTrackerContext : DbContext
{
    /// <summary>
    ///     Инициализирует контекст с параметрами по умолчанию (подключение из <see cref="OnConfiguring" />).
    /// </summary>
    public BusinessTrackerContext()
    {
    }

    /// <summary>
    ///     Инициализирует контекст с явно переданными параметрами подключения.
    /// </summary>
    /// <param name="options">Параметры DbContext.</param>
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

    public virtual DbSet<JournalRow> JournalRows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(
                "User ID=admin;Password=123456;Host=localhost;Port=5433;Database=business_tracker;");
    }

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

        modelBuilder.Entity<JournalRow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("JournalRows_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.TransTypeName).HasDefaultValue(string.Empty);
            entity.Property(e => e.EmployeeName).HasDefaultValue(string.Empty);
            entity.Property(e => e.CategoryName).HasDefaultValue(string.Empty);
            entity.Property(e => e.NomenclatureName).HasDefaultValue(string.Empty);
            entity.Property(e => e.Period).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Owner).WithMany()
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_JournalRows_Organizations");
        });

        SeedData(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var org1Id = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
        var org2Id = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a12");
        var userId = new Guid("b0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
        var categoryId = new Guid("c1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
        var nomenclatureId = new Guid("d1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
        var employeeId = new Guid("e1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");

        modelBuilder.Entity<Organization>().HasData(
            new Organization
            {
                Id = org1Id,
                Name = "Главный офис (Спб)",
                Inn = "1234567890",
                Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
            },
            new Organization
            {
                Id = org2Id,
                Name = "Филиал (Иркутск)",
                Inn = "0987654321",
                Address = "230000, Иркутская обл., Октябрьский р-н, г. Иркутск, ул. 3 Июля, д. 3"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = userId, Name = "Администратор", Password = "admin123" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = categoryId, Name = "Продукты питания", OwnerId = org1Id }
        );

        modelBuilder.Entity<Nomenclature>().HasData(
            new Nomenclature { Id = nomenclatureId, Name = "Хлеб пшеничный", CategoryId = categoryId }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = employeeId,
                Name = "Иван Иванов",
                PhoneNumber = "+79991234567",
                OwnerId = org1Id,
                Role = 1
            }
        );
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}