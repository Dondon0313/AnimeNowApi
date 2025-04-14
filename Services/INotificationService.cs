using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeNowApi.DTOs;  

namespace AnimeNowApi.Services
{
    public interface INotificationService
    {
        Task CreateNotification(int userId, string type, string message, int? relatedId = null);
        Task<List<NotificationDto>> GetUserNotifications(int userId, bool unreadOnly = false);
        Task MarkAsRead(int notificationId, int userId);
        Task MarkAllAsRead(int userId);
    }
}