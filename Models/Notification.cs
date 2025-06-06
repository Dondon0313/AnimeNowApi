﻿namespace AnimeNowApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Type { get; set; } 
        public required string Message { get; set; }
        public int? RelatedId { get; set; } 
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public required User User { get; set; }
    }
}