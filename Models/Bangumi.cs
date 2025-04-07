﻿using System;
using System.Collections.Generic;

namespace AnimeNowApi.Models
{
    public class Bangumi
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public DateTime AirDate { get; set; }
        public string WeekDay { get; set; }
        public int TotalEpisodes { get; set; }
        public string Studio { get; set; }
        public decimal Rating { get; set; }
        public string Status { get; set; } // upcoming, ongoing, completed

        // 導航屬性
        public ICollection<Episode> Episodes { get; set; }
        public ICollection<BangumiGenre> BangumiGenres { get; set; }
    }
}
