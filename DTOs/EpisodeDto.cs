using System;
namespace AnimeNowApi.DTOs
{
    public class EpisodeDto
    {
        public int Id { get; set; }
        public int BangumiId { get; set; }
        public int Number { get; set; }
        public required string Title { get; set; }
        public DateTime AirDate { get; set; }
        public required string Duration { get; set; }
        public required string Description { get; set; }
        public required string Thumbnail { get; set; }
        public int Views { get; set; }
    }
}