// AnimeDbContext.cs (更新)
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
        public DbSet<Favorite> Favorite { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<WatchHistory> WatchHistory { get; set; }
        public DbSet<Friendship> Friendship { get; set; }
        public DbSet<Notification> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 現有配置
            modelBuilder.Entity<BangumiGenre>()
                .HasKey(bg => new { bg.BangumiId, bg.GenreId });

            modelBuilder.Entity<BangumiGenre>()
                .HasOne(bg => bg.Bangumi)
                .WithMany(b => b.BangumiGenres)
                .HasForeignKey(bg => bg.BangumiId);

            modelBuilder.Entity<BangumiGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BangumiGenres)
                .HasForeignKey(bg => bg.GenreId);

            modelBuilder.Entity<Episode>()
                .HasOne(e => e.Bangumi)
                .WithMany(b => b.Episodes)
                .HasForeignKey(e => e.BangumiId);

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

            // 好友關係配置
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Requester)
                .WithMany()
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Addressee)
                .WithMany()
                .HasForeignKey(f => f.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 通知配置
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId);
        }
    }
}