// WatchHistoryController.cs
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
    public class WatchHistoryController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        private readonly IMapper _mapper;

        public WatchHistoryController(AnimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/WatchHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WatchHistoryDto>>> GetUserWatchHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var history = await _context.WatchHistory
                .Where(h => h.UserId == userId)
                .Include(h => h.Episode)
                .ThenInclude(e => e.Bangumi)
                .OrderByDescending(h => h.LastWatched)
                .ToListAsync();

            return history.Select(h => new WatchHistoryDto
            {
                Id = h.Id,
                EpisodeId = h.EpisodeId,
                BangumiId = h.Episode.BangumiId,
                EpisodeNumber = h.Episode.Number,
                EpisodeTitle = h.Episode.Title,
                BangumiTitle = h.Episode.Bangumi.Title,
                Progress = h.Progress,
                LastWatched = h.LastWatched
            }).ToList();
        }

        // POST: api/WatchHistory
        [HttpPost]
        public async Task<ActionResult<WatchHistoryDto>> AddWatchHistory(CreateWatchHistoryDto watchDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 檢查劇集是否存在
            var episode = await _context.Episodes
                .Include(e => e.Bangumi)
                .FirstOrDefaultAsync(e => e.Id == watchDto.EpisodeId);

            if (episode == null)
            {
                return NotFound("劇集不存在");
            }

            // 查找現有記錄，如果存在則更新
            var existingHistory = await _context.WatchHistory
                .FirstOrDefaultAsync(h => h.UserId == userId && h.EpisodeId == watchDto.EpisodeId);

            if (existingHistory != null)
            {
                existingHistory.Progress = watchDto.Progress;
                existingHistory.LastWatched = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new WatchHistoryDto
                {
                    Id = existingHistory.Id,
                    EpisodeId = existingHistory.EpisodeId,
                    BangumiId = episode.BangumiId,
                    EpisodeNumber = episode.Number,
                    EpisodeTitle = episode.Title,
                    BangumiTitle = episode.Bangumi.Title,
                    Progress = existingHistory.Progress,
                    LastWatched = existingHistory.LastWatched
                });
            }

            // 添加新記錄
            var watchHistory = new WatchHistory
            {
                UserId = userId,
                EpisodeId = watchDto.EpisodeId,
                Progress = watchDto.Progress,
                LastWatched = DateTime.Now,
                User = await _context.Users.FindAsync(userId),
                Episode = episode
            };

            _context.WatchHistory.Add(watchHistory);
            await _context.SaveChangesAsync();

            return Ok(new WatchHistoryDto
            {
                Id = watchHistory.Id,
                EpisodeId = watchHistory.EpisodeId,
                BangumiId = episode.BangumiId,
                EpisodeNumber = episode.Number,
                EpisodeTitle = episode.Title,
                BangumiTitle = episode.Bangumi.Title,
                Progress = watchHistory.Progress,
                LastWatched = watchHistory.LastWatched
            });
        }

        // DELETE: api/WatchHistory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWatchHistory(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var history = await _context.WatchHistory
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (history == null)
            {
                return NotFound("歷史記錄不存在");
            }

            _context.WatchHistory.Remove(history);
            await _context.SaveChangesAsync();

            return Ok("已刪除觀看記錄");
        }

        // DELETE: api/WatchHistory/ClearAll
        [HttpDelete("ClearAll")]
        public async Task<IActionResult> ClearAllWatchHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var historyRecords = await _context.WatchHistory
                .Where(h => h.UserId == userId)
                .ToListAsync();

            _context.WatchHistory.RemoveRange(historyRecords);
            await _context.SaveChangesAsync();

            return Ok("已清除所有觀看記錄");
        }
    }
}