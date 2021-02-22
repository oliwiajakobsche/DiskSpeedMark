using Microsoft.EntityFrameworkCore;

namespace DiskSpeedMark.Database
{
    class ResultsDbContext : DbContext
    {
        public DbSet<TestResult> TestsResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=BenchmarkDb.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
