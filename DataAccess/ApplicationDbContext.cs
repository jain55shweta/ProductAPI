using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=IMW1031317C;Database=ProductDb;Trusted_Connection=True;", options =>
                    options.MigrationsAssembly("ProductAPI")); // Specify the target project here
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);  // Specifies precision = 18 and scale = 2

            modelBuilder.Entity<Product>()
           .Property(p => p.ProductId)
           .ValueGeneratedNever();


        }
    }
}
