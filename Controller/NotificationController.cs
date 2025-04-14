// NotificationController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AnimeNowApi.Services;
using AnimeNowApi.DTOs;
using System.Security.Claims;

namespace AnimeNowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications([FromQuery] bool unreadOnly = false)
        {
            // 安全地解析 UserId
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(); // 如果無法取得有效的使用者 ID
            }

            var notifications = await _notificationService.GetUserNotifications(userId, unreadOnly);

            return notifications == null || !notifications.Any()
                ? NoContent()
                : Ok(notifications);
        }

        // PUT: api/Notification/5/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _notificationService.MarkAsRead(id, userId);
            return Ok();
        }

        // PUT: api/Notification/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _notificationService.MarkAllAsRead(userId);
            return Ok();
        }
    }
}