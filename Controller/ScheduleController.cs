using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Data;
using AnimeNowApi.DTOs;
using AutoMapper;

namespace AnimeNowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        private readonly IMapper _mapper;

        public ScheduleController(AnimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Schedule
        [HttpGet]
        public async Task<ActionResult<Dictionary<string, IEnumerable<BangumiDto>>>> GetWeeklySchedule()
        {
            var weekdays = new[] { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日" };

            var result = new Dictionary<string, IEnumerable<BangumiDto>>();

            // 只獲取進行中的番劇
            var bangumis = await _context.Bangumis
                .Where(b => b.Status == "ongoing")
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .ToListAsync();

            foreach (var weekday in weekdays)
            {
                var dayBangumis = bangumis.Where(b => b.WeekDay == weekday).ToList();
                result[weekday] = _mapper.Map<List<BangumiDto>>(dayBangumis);
            }

            return result;
        }

        // GET: api/Schedule/Today
        [HttpGet("Today")]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetTodaySchedule()
        {
            // 獲取當天的星期
            var today = DateTime.Now.DayOfWeek;
            var weekdayMap = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "星期一" },
                { DayOfWeek.Tuesday, "星期二" },
                { DayOfWeek.Wednesday, "星期三" },
                { DayOfWeek.Thursday, "星期四" },
                { DayOfWeek.Friday, "星期五" },
                { DayOfWeek.Saturday, "星期六" },
                { DayOfWeek.Sunday, "星期日" }
            };

            var todayWeekday = weekdayMap[today];

            // 獲取今天播出的番劇
            var bangumis = await _context.Bangumis
                .Where(b => b.Status == "ongoing" && b.WeekDay == todayWeekday)
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .ToListAsync();

            return _mapper.Map<List<BangumiDto>>(bangumis);
        }

        // GET: api/Schedule/Upcoming
        [HttpGet("Upcoming")]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetUpcomingBangumis()
        {
            // 獲取即將播出的番劇（未來30天內）
            var startDate = DateTime.Now;
            var endDate = startDate.AddDays(30);

            var bangumis = await _context.Bangumis
                .Where(b => b.Status == "upcoming" && b.AirDate >= startDate && b.AirDate <= endDate)
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .OrderBy(b => b.AirDate)
                .ToListAsync();

            return _mapper.Map<List<BangumiDto>>(bangumis);
        }
    }
}