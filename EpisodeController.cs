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
    public class EpisodeController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        private readonly IMapper _mapper;

        public EpisodeController(AnimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Episode
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EpisodeDto>>> GetEpisodes()
        {
            var episodes = await _context.Episodes.ToListAsync();
            return _mapper.Map<List<EpisodeDto>>(episodes);
        }

        // GET: api/Episode/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EpisodeDto>> GetEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);

            if (episode == null)
            {
                return NotFound();
            }

            return _mapper.Map<EpisodeDto>(episode);
        }

        // GET: api/Episode/Bangumi/5
        [HttpGet("Bangumi/{bangumiId}")]
        public async Task<ActionResult<IEnumerable<EpisodeDto>>> GetEpisodesByBangumi(int bangumiId)
        {
            var episodes = await _context.Episodes
                .Where(e => e.BangumiId == bangumiId)
                .OrderBy(e => e.Number)
                .ToListAsync();

            return _mapper.Map<List<EpisodeDto>>(episodes);
        }

        // POST: api/Episode/5/IncrementViews
        [HttpPost("{id}/IncrementViews")]
        public async Task<IActionResult> IncrementViews(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);

            if (episode == null)
            {
                return NotFound();
            }

            episode.Views++;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}