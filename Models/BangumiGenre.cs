namespace AnimeNowApi.Models
{
    public class BangumiGenre
    {
        public int BangumiId { get; set; }
        public required Bangumi Bangumi { get; set; }
        public int GenreId { get; set; }
        public required Genre Genre { get; set; }
    }
}