using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Data;
using AnimeNowApi.Models;
using AnimeNowApi.DTOs;
using System.Security.Claims;

namespace AnimeNowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AnimeDbContext _context;

        public CommentController(AnimeDbContext context)
        {
            _context = context;
        }

        // GET: api/Comment/Bangumi/5
        [HttpGet("Bangumi/{bangumiId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetBangumiComments(int bangumiId)
        {
            var comments = await _context.Comment
                .Where(c => c.TargetId == bangumiId && c.TargetType == "bangumi")
                .Include(c => c.User)
                .OrderByDescending(c => c.Created)
                .ToListAsync();

            return comments.Select(c => new CommentDto
            {
                Id = c.Id,
                TargetId = c.TargetId,
                TargetType = c.TargetType,
                Content = c.Content,
                Created = c.Created,
                Username = c.User.Username
            }).ToList();
        }

        // GET: api/Comment/Episode/5
        [HttpGet("Episode/{episodeId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetEpisodeComments(int episodeId)
        {
            var comments = await _context.Comment
                .Where(c => c.TargetId == episodeId && c.TargetType == "episode")
                .Include(c => c.User)
                .OrderByDescending(c => c.Created)
                .ToListAsync();

            return comments.Select(c => new CommentDto
            {
                Id = c.Id,
                TargetId = c.TargetId,
                TargetType = c.TargetType,
                Content = c.Content,
                Created = c.Created,
                Username = c.User.Username
            }).ToList();
        }

        // POST: api/Comment
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDto>> PostComment(CreateCommentDto commentDto)
        {
            // 安全地解析使用者 ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(); // 如果無法取得有效的使用者 ID
            }

            // 查詢使用者
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var comment = new Comment
            {
                UserId = userId,
                User = user,  // 確保設置 User 屬性
                TargetId = commentDto.TargetId,
                TargetType = commentDto.TargetType,
                Content = commentDto.Content,
                Created = DateTime.Now
            };

            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBangumiComments), new { bangumiId = comment.TargetId }, new CommentDto
            {
                Id = comment.Id,
                TargetId = comment.TargetId,
                TargetType = comment.TargetType,
                Content = comment.Content,
                Created = comment.Created,
                Username = user.Username
            });
        }

        // DELETE: api/Comment/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var comment = await _context.Comment.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != userId)
            {
                return Forbid();
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
