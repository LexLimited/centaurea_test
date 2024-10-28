using CentaureaTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CentaureaTest.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<GridsTable> Grids { get; set; }
    public DbSet<FieldsTable> Fields { get; set; }
    public DbSet<DataGridValue> Values { get; set; }
    public DbSet<SingleSelectTable> SingleSelectOptions { get; set; }
    public DbSet<MultiSelectTable> MultiSelectOptions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            var configuration = new ConfigurationBuilder().Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            options.UseNpgsql(connectionString);
        }

        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<FieldsTable>()
            .HasDiscriminator<string>("FieldType")
            .HasValue<FieldsTable>("Base")
            .HasValue<RegexFieldsTable>("Regex")
            .HasValue<RefFieldsTable>("Ref");

        builder.Entity<DataGridValue>()
            .HasDiscriminator<string>("ValueType")
            .HasValue<DataGridValue>("Base")
            .HasValue<DataGridNumericValue>("Numeric")
            .HasValue<DataGridStringValue>("String")
            .HasValue<DataGridRegexValue>("Regex")
            .HasValue<DataGridRefValue>("Ref")
            .HasValue<DataGridSingleSelectValue>("SingleSelect")
            .HasValue<DataGridMultiSelectValue>("MultiSelect");

        base.OnModelCreating(builder);
    }

}