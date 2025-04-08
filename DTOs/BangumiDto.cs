using System;
using System.Collections.Generic;
namespace AnimeNowApi.DTOs
{
    public class BangumiDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Image { get; set; }
        public required string Description { get; set; }
        public DateTime AirDate { get; set; }
        public required string WeekDay { get; set; }
        public int TotalEpisodes { get; set; }
        public required List<string> Genres { get; set; }
        public decimal Rating { get; set; }
        public required string Studio { get; set; }
        public required string Status { get; set; }
    }
}