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
    public class UserController : ControllerBase
    {
        private readonly AnimeDbContext _context;

        public UserController(AnimeDbContext context)
        {
            _context = context;
        }

        // GET: api/User/Search
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("請提供用戶名");
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var users = await _context.Users
                .Where(u => u.Username.Contains(username) && u.Id != currentUserId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .Take(10)
                .ToListAsync();

            return users;
        }
    }
}