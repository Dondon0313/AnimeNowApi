// FavoriteController.cs
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
using AutoMapper;

namespace AnimeNowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        private readonly IMapper _mapper;

        public FavoriteController(AnimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Favorite
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetUserFavorites()
        {
            // 安全地解析 UserId
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(); // 或者適當的錯誤處理
            }

            var favorites = await _context.Favorite
                .Where(f => f.UserId == userId)
                .Include(f => f.Bangumi)
                    .ThenInclude(b => b.BangumiGenres)
                        .ThenInclude(bg => bg.Genre)
                .Select(f => f.Bangumi)
                .ToListAsync();

            return favorites == null || !favorites.Any()
                ? NoContent()
                : Ok(_mapper.Map<List<BangumiDto>>(favorites));
        }

        // POST: api/Favorite
        [HttpPost("{bangumiId}")]
        public async Task<IActionResult> AddFavorite(int bangumiId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 檢查番劇是否存在
            var bangumi = await _context.Bangumis.FindAsync(bangumiId);
            if (bangumi == null)
            {
                return NotFound("番劇不存在");
            }

            // 檢查是否已經收藏
            var existingFavorite = await _context.Favorite
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BangumiId == bangumiId);

            if (existingFavorite != null)
            {
                return BadRequest("已經收藏過此番劇");
            }

            var favorite = new Favorite
            {
                UserId = userId,
                BangumiId = bangumiId,
                User = await _context.Users.FindAsync(userId),
                Bangumi = bangumi
            };

            _context.Favorite.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok("收藏成功");
        }

        // DELETE: api/Favorite/5
        [HttpDelete("{bangumiId}")]
        public async Task<IActionResult> RemoveFavorite(int bangumiId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var favorite = await _context.Favorite
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BangumiId == bangumiId);

            if (favorite == null)
            {
                return NotFound("未找到此收藏");
            }

            _context.Favorite.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok("已取消收藏");
        }
    }
}