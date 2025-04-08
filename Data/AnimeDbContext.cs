using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Models;

namespace AnimeNowApi.Data
{
    public class AnimeDbContext : DbContext
    {
        public AnimeDbContext(DbContextOptions<AnimeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bangumi> Bangumis { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BangumiGenre> BangumiGenres { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 BangumiGenre 
            modelBuilder.Entity<BangumiGenre>()
                .HasKey(bg => new { bg.BangumiId, bg.GenreId });

            // 配置 Bangumi 和 BangumiGenre 的關係
            modelBuilder.Entity<BangumiGenre>()
                .HasOne(bg => bg.Bangumi)
                .WithMany(b => b.BangumiGenres)
                .HasForeignKey(bg => bg.BangumiId);

            // 配置 Genre 和 BangumiGenre 的關係
            modelBuilder.Entity<BangumiGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BangumiGenres)
                .HasForeignKey(bg => bg.GenreId);

            // 配置 Bangumi 和 Episode 的一對多關係
            modelBuilder.Entity<Episode>()
                .HasOne(e => e.Bangumi)
                .WithMany(b => b.Episodes)
                .HasForeignKey(e => e.BangumiId);

            // 設置 Bangumi Rating 屬性的精度 
            modelBuilder.Entity<Bangumi>()
                .Property(b => b.Rating)
                .HasPrecision(3, 1);

            // 用戶配置
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}