using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Data;
using AnimeNowApi.Models;
using AnimeNowApi.DTOs;
using System.Security.Claims;

namespace AnimeNowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AdminController : ControllerBase
    {
        private readonly AnimeDbContext _context;

        public AdminController(AnimeDbContext context)
        {
            _context = context;
        }

        // POST: api/Admin/Bangumi
        [HttpPost("Bangumi")]
        public async Task<ActionResult<Bangumi>> CreateBangumi(Bangumi bangumi)
        {
            _context.Bangumis.Add(bangumi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBangumi", new { id = bangumi.Id }, bangumi);
        }

        // PUT: api/Admin/Bangumi/5
        [HttpPut("Bangumi/{id}")]
        public async Task<IActionResult> UpdateBangumi(int id, Bangumi bangumi)
        {
            if (id != bangumi.Id)
            {
                return BadRequest();
            }

            _context.Entry(bangumi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BangumiExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Admin/Bangumi/5
        [HttpDelete("Bangumi/{id}")]
        public async Task<IActionResult> DeleteBangumi(int id)
        {
            var bangumi = await _context.Bangumis.FindAsync(id);
            if (bangumi == null)
            {
                return NotFound();
            }

            _context.Bangumis.Remove(bangumi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 增加劇集
        [HttpPost("Episode")]
        public async Task<ActionResult<Episode>> CreateEpisode(Episode episode)
        {
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEpisode", new { id = episode.Id }, episode);
        }

        // 更新劇集
        [HttpPut("Episode/{id}")]
        public async Task<IActionResult> UpdateEpisode(int id, Episode episode)
        {
            if (id != episode.Id)
            {
                return BadRequest();
            }

            _context.Entry(episode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // 刪除劇集
        [HttpDelete("Episode/{id}")]
        public async Task<IActionResult> DeleteEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }

            _context.Episodes.Remove(episode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BangumiExists(int id)
        {
            return _context.Bangumis.Any(e => e.Id == id);
        }

        private bool EpisodeExists(int id)
        {
            return _context.Episodes.Any(e => e.Id == id);
        }
    }
}