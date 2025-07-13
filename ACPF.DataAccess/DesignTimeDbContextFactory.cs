using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ACPF.DataAccess
{
    /// <summary>
    /// Factory para criar o DbContext durante o design-time (para migrations)
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=financas.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
} 