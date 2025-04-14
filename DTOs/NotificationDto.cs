// NotificationDto.cs (在 DTOs 資料夾下創建此檔案)
using System;

namespace AnimeNowApi.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public required string Message { get; set; }
        public int? RelatedId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}