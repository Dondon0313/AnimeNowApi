using System.Collections.Generic;

namespace AnimeNowApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        public ICollection<BangumiGenre> BangumiGenres { get; set; } = new List<BangumiGenre>();
    }
}