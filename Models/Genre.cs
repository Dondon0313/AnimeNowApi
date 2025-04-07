using System.Collections.Generic;

namespace AnimeNowApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 導航屬性
        public ICollection<BangumiGenre> BangumiGenres { get; set; }
    }
}