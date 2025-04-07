namespace AnimeNowApi.Models
{
    public class BangumiGenre
    {
        public int BangumiId { get; set; }
        public Bangumi Bangumi { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}