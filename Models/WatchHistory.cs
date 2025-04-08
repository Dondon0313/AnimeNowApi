namespace AnimeNowApi.Models
{
    public class WatchHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EpisodeId { get; set; }
        public int Progress { get; set; } 
        public DateTime LastWatched { get; set; } = DateTime.Now;

        public required User User { get; set; }
        public required Episode Episode { get; set; }
    }
}