using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Data;
using AnimeNowApi.Models;
using AnimeNowApi.DTOs;
using System;

namespace AnimeNowApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AnimeDbContext _context;

        public NotificationService(AnimeDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotification(int userId, string type, string message, int? relatedId = null)
        {
            // 先查詢用戶
            var user = await _context.Users.FindAsync(userId);

            // 檢查用戶是否存在
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Message = message,
                RelatedId = relatedId,
                User = user  // 使用查詢到的用戶
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationDto>> GetUserNotifications(int userId, bool unreadOnly = false)
        {
            var query = _context.Notification.Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Message = n.Message,
                RelatedId = n.RelatedId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task MarkAsRead(int notificationId, int userId)
        {
            var notification = await _context.Notification
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead(int userId)
        {
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}