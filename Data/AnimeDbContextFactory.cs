using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AnimeNowApi.Data
{
    public class AnimeDbContextFactory : IDesignTimeDbContextFactory<AnimeDbContext>
    {
        public AnimeDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnimeDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AnimeNowDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new AnimeDbContext(optionsBuilder.Options);
        }
    }
}