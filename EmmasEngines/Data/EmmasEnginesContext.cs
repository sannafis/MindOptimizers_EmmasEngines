using Microsoft.EntityFrameworkCore;

namespace EmmasEngines.Data
{
    public class EmmasEnginesContext : DbContext
    {
        public EmmasEnginesContext(DbContextOptions<EmmasEnginesContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
