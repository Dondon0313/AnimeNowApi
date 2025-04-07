using System;

namespace AnimeNowApi.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public int BangumiId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public DateTime AirDate { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public string? Thumbnail { get; set; } 
        public string? VideoUrl { get; set; }  
        public int Views { get; set; }

        // 導航屬性
        public Bangumi Bangumi { get; set; }
    }
}