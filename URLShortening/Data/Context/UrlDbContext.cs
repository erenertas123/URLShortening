
using Microsoft.EntityFrameworkCore;
using URLShortening.Entity;


namespace URLShortening.Data {
    public class UrlDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public UrlDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("UrlDatabase"));
        }

        public DbSet<URL> Urls { get; set; } = null!;
    }
}
