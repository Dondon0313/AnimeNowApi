using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AnimeNowApi.Data
{
    public class AnimeDbContextFactory : IDesignTimeDbContextFactory<AnimeDbContext>
    {
        public AnimeDbContext CreateDbContext(string[] args)
        {
            // 加載配置
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AnimeDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new AnimeDbContext(optionsBuilder.Options);
        }
    }
}