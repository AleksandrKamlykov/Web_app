using Microsoft.EntityFrameworkCore;
using Web_app.Models;

namespace Web_app.DBContext;

public class MainDBContext : DbContext
{
    public MainDBContext(DbContextOptions<MainDBContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = "Server=tcp:test-step.database.windows.net,1433;Initial Catalog=test-step;Persist Security Info=False;User ID=alex_admin;Password=181194kam.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Additional model configuration
    }
}