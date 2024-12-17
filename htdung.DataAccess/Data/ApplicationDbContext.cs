using Microsoft.EntityFrameworkCore;

namespace htdung.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // Override OnModelCreating to allow for custom entity configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Here you can add custom configurations if necessary, e.g., indexes, relationships, etc.
        }

        // This method is used to get a DbSet of any entity dynamically
        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }
    }
}
