using CentaureaTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CentaureaTest.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<GridsTable> Grids { get; set; }
    public DbSet<FieldsTable> Fields { get; set; }
    public DbSet<ValuesTable> Values { get; set; }
    public DbSet<SingleChoiceTable> SingleChoiceOptions { get; set; }
    public DbSet<MultipleChoiceTable> MultipleChoiceOptions { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {}

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
}