namespace AnimeNowApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TargetId { get; set; }
        public required string TargetType { get; set; }
        public required string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public required User User { get; set; }
    }
}