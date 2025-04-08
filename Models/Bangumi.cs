using System;
using System.Collections.Generic;

namespace AnimeNowApi.Models
{
    public class Bangumi
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Image { get; set; }
        public required string Description { get; set; }
        public DateTime AirDate { get; set; }
        public required string WeekDay { get; set; }
        public int TotalEpisodes { get; set; }
        public required string Studio { get; set; }
        public decimal Rating { get; set; }
        public required string Status { get; set; } 

       
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
        public ICollection<BangumiGenre> BangumiGenres { get; set; } = new List<BangumiGenre>();
    }

}