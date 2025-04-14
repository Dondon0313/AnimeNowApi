using System;
namespace AnimeNowApi.DTOs
{
    public class WatchHistoryDto
    {
        public int Id { get; set; }
        public int EpisodeId { get; set; }
        public int BangumiId { get; set; }
        public int EpisodeNumber { get; set; }
        public required string EpisodeTitle { get; set; }
        public required string BangumiTitle { get; set; }
        public int Progress { get; set; }
        public DateTime LastWatched { get; set; }
    }

    public class CreateWatchHistoryDto
    {
        public int EpisodeId { get; set; }
        public int Progress { get; set; }
    }
}