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
    public class RecommendationController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        private readonly IMapper _mapper;

        public RecommendationController(AnimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Recommendation
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetPersonalizedRecommendations()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 從用戶觀看歷史和收藏獲取喜好的類型
            var userFavoriteGenres = await GetUserPreferredGenres(userId);

            if (!userFavoriteGenres.Any())
            {
                // 如果沒有足夠數據，則返回評分最高的番劇
                var topRated = await _context.Bangumis
                    .Include(b => b.BangumiGenres)
                    .ThenInclude(bg => bg.Genre)
                    .OrderByDescending(b => b.Rating)
                    .Take(10)
                    .ToListAsync();

                return _mapper.Map<List<BangumiDto>>(topRated);
            }

            // 基於用戶喜好的類型推薦番劇
            var recommendations = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => b.BangumiGenres.Any(bg => userFavoriteGenres.Contains(bg.Genre.Name)))
                .OrderByDescending(b => b.Rating)
                .Take(10)
                .ToListAsync();

            // 排除用戶已經收藏的番劇
            var userFavorites = await _context.Favorite
                .Where(f => f.UserId == userId)
                .Select(f => f.BangumiId)
                .ToListAsync();

            var filtered = recommendations
                .Where(b => !userFavorites.Contains(b.Id))
                .ToList();

            return _mapper.Map<List<BangumiDto>>(filtered);
        }

        // GET: api/Recommendation/Similar/5
        [HttpGet("Similar/{bangumiId}")]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetSimilarBangumis(int bangumiId)
        {
            // 檢查番劇是否存在
            var bangumi = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .FirstOrDefaultAsync(b => b.Id == bangumiId);

            if (bangumi == null)
            {
                return NotFound("番劇不存在");
            }

            // 獲取該番劇的類型
            var genres = bangumi.BangumiGenres.Select(bg => bg.Genre.Name).ToList();

            // 找到類型相似的番劇
            var similar = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => b.Id != bangumiId && b.BangumiGenres.Any(bg => genres.Contains(bg.Genre.Name)))
                .OrderByDescending(b => b.Rating)
                .Take(5)
                .ToListAsync();

            return _mapper.Map<List<BangumiDto>>(similar);
        }

        // 獲取用戶偏好的類型
        private async Task<List<string>> GetUserPreferredGenres(int userId)
        {
            // 從用戶收藏的番劇中獲取類型
            var favoriteGenres = await _context.Favorite
                .Where(f => f.UserId == userId)
                .Select(f => f.Bangumi)
                .SelectMany(b => b.BangumiGenres.Select(bg => bg.Genre.Name))
                .ToListAsync();

            // 從用戶觀看歷史獲取類型
            var watchedGenres = await _context.WatchHistory
                .Where(h => h.UserId == userId)
                .Select(h => h.Episode.Bangumi)
                .SelectMany(b => b.BangumiGenres.Select(bg => bg.Genre.Name))
                .ToListAsync();

            // 結合並計算出現頻率
            var allGenres = favoriteGenres.Concat(watchedGenres);
            var genreCounts = allGenres
                .GroupBy(g => g)
                .Select(g => new { Genre = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Select(g => g.Genre)
                .ToList();

            return genreCounts;
        }

        // GET: api/Recommendation/Trending
        [HttpGet("Trending")]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetTrendingBangumis()
        {
            // 根據最近觀看次數和評論量獲取熱門番劇
            var twoWeeksAgo = DateTime.Now.AddDays(-14);

            // 獲取最近兩週內的觀看記錄數量
            var recentViews = await _context.WatchHistory
                .Where(h => h.LastWatched >= twoWeeksAgo)
                .GroupBy(h => h.Episode.BangumiId)
                .Select(g => new { BangumiId = g.Key, ViewCount = g.Count() })
                .ToListAsync();

            // 獲取最近兩週內的評論數量
            var recentComments = await _context.Comment
                .Where(c => c.Created >= twoWeeksAgo && c.TargetType == "bangumi")
                .GroupBy(c => c.TargetId)
                .Select(g => new { BangumiId = g.Key, CommentCount = g.Count() })
                .ToListAsync();

            // 合併數據計算趨勢分數
            var trendScores = new Dictionary<int, double>();

            foreach (var view in recentViews)
            {
                trendScores[view.BangumiId] = view.ViewCount;
            }

            foreach (var comment in recentComments)
            {
                if (trendScores.ContainsKey(comment.BangumiId))
                {
                    trendScores[comment.BangumiId] += comment.CommentCount * 2; 
                }
                else
                {
                    trendScores[comment.BangumiId] = comment.CommentCount * 2;
                }
            }

            // 根據趨勢分數獲取番劇
            var trendingIds = trendScores.OrderByDescending(s => s.Value)
                .Take(10)
                .Select(s => s.Key)
                .ToList();

            if (!trendingIds.Any())
            {
                // 如果沒有足夠的數據，返回熱門番劇
                var popular = await _context.Bangumis
                    .Include(b => b.BangumiGenres)
                    .ThenInclude(bg => bg.Genre)
                    .OrderByDescending(b => b.Rating)
                    .Take(10)
                    .ToListAsync();

                return _mapper.Map<List<BangumiDto>>(popular);
            }

            var trending = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => trendingIds.Contains(b.Id))
                .ToListAsync();

            // 按趨勢分數排序
            var ordered = trending.OrderBy(b => trendingIds.IndexOf(b.Id)).ToList();

            return _mapper.Map<List<BangumiDto>>(ordered);
        }
    }
}