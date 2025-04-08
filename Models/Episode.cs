using System;

namespace AnimeNowApi.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public int BangumiId { get; set; }
        public int Number { get; set; }
        public required string Title { get; set; }
        public DateTime AirDate { get; set; }
        public required string Duration { get; set; }
        public required string Description { get; set; }
        public string? Thumbnail { get; set; }
        public string? VideoUrl { get; set; }
        public int Views { get; set; }

    
        public required Bangumi Bangumi { get; set; }
    }
}