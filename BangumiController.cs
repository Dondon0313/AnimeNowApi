using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AnimeNowApi.Data;
using AnimeNowApi.Models;
using AnimeNowApi.DTOs;

namespace AnimeNowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BangumiController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        private readonly IMapper _mapper;

        public BangumiController(AnimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Bangumi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetBangumis()
        {
            var bangumis = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .ToListAsync();

            return _mapper.Map<List<BangumiDto>>(bangumis);
        }

        // GET: api/Bangumi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BangumiDto>> GetBangumi(int id)
        {
            var bangumi = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .Include(b => b.Episodes)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bangumi == null)
            {
                return NotFound();
            }

            return _mapper.Map<BangumiDto>(bangumi);
        }

        // GET: api/Bangumi/Status/ongoing
        [HttpGet("Status/{status}")]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetBangumisByStatus(string status)
        {
            var bangumis = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => b.Status == status)
                .ToListAsync();

            return _mapper.Map<List<BangumiDto>>(bangumis);
        }

        // GET: api/Bangumi/WeekDay/星期一
        [HttpGet("WeekDay/{weekDay}")]
        public async Task<ActionResult<IEnumerable<BangumiDto>>> GetBangumisByWeekDay(string weekDay)
        {
            var bangumis = await _context.Bangumis
                .Include(b => b.BangumiGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => b.WeekDay == weekDay)
                .ToListAsync();

            return _mapper.Map<List<BangumiDto>>(bangumis);
        }
    }
}