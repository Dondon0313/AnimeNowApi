// Notification.cs
namespace AnimeNowApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Type { get; set; } // "friend_request", "episode_update", "comment_reply", etc.
        public required string Message { get; set; }
        public int? RelatedId { get; set; } // 相關實體的ID，例如：番劇ID，評論ID等
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public required User User { get; set; }
    }
}