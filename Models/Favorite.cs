namespace AnimeNowApi.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BangumiId { get; set; }
        public required User User { get; set; }
        public required Bangumi Bangumi { get; set; }
    }
}