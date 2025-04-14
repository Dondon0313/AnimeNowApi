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
    [Authorize]
    public class FriendshipController : ControllerBase
    {
        private readonly AnimeDbContext _context;

        public FriendshipController(AnimeDbContext context)
        {
            _context = context;
        }

        // GET: api/Friendship
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetFriends()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 獲取已接受的朋友關係
            var friendships = await _context.Friendship
                .Where(f =>
                    (f.RequesterId == userId || f.AddresseeId == userId) &&
                    f.Status == "accepted")
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .ToListAsync();

            var result = new List<FriendshipDto>();

            foreach (var friendship in friendships)
            {
                // 根據當前用戶ID確定好友是誰
                if (friendship.RequesterId == userId)
                {
                    result.Add(new FriendshipDto
                    {
                        Id = friendship.Id,
                        UserId = friendship.AddresseeId,
                        Username = friendship.Addressee.Username,
                        Status = friendship.Status,
                        CreatedAt = friendship.CreatedAt
                    });
                }
                else
                {
                    result.Add(new FriendshipDto
                    {
                        Id = friendship.Id,
                        UserId = friendship.RequesterId,
                        Username = friendship.Requester.Username,
                        Status = friendship.Status,
                        CreatedAt = friendship.CreatedAt
                    });
                }
            }

            return result;
        }

        // GET: api/Friendship/Requests
        [HttpGet("Requests")]
        public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetFriendRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 獲取發送給當前用戶的待處理請求
            var requests = await _context.Friendship
                .Where(f => f.AddresseeId == userId && f.Status == "pending")
                .Include(f => f.Requester)
                .ToListAsync();

            return requests.Select(r => new FriendshipDto
            {
                Id = r.Id,
                UserId = r.RequesterId,
                Username = r.Requester.Username,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        // POST: api/Friendship
        [HttpPost]
        public async Task<ActionResult<FriendshipDto>> SendFriendRequest(FriendRequestDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 檢查是否為自己
            if (userId == request.AddresseeId)
            {
                return BadRequest("不能添加自己為好友");
            }

            // 檢查對方用戶是否存在
            var addressee = await _context.Users.FindAsync(request.AddresseeId);
            if (addressee == null)
            {
                return NotFound("用戶不存在");
            }

            // 檢查是否已經是好友或有待處理的請求
            var existingFriendship = await _context.Friendship
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == userId && f.AddresseeId == request.AddresseeId) ||
                    (f.RequesterId == request.AddresseeId && f.AddresseeId == userId));

            if (existingFriendship != null)
            {
                if (existingFriendship.Status == "accepted")
                {
                    return BadRequest("已經是好友");
                }
                if (existingFriendship.Status == "pending")
                {
                    return BadRequest("好友請求已發送或等待您的回應");
                }
            }

            var friendship = new Friendship
            {
                RequesterId = userId,
                AddresseeId = request.AddresseeId,
                Status = "pending",
                Requester = await _context.Users.FindAsync(userId),
                Addressee = addressee
            };

            _context.Friendship.Add(friendship);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFriends), new FriendshipDto
            {
                Id = friendship.Id,
                UserId = friendship.AddresseeId,
                Username = friendship.Addressee.Username,
                Status = friendship.Status,
                CreatedAt = friendship.CreatedAt
            });
        }

        // PUT: api/Friendship/5/accept
        [HttpPut("{id}/accept")]
        public async Task<IActionResult> AcceptFriendRequest(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var friendship = await _context.Friendship
                .FirstOrDefaultAsync(f => f.Id == id && f.AddresseeId == userId && f.Status == "pending");

            if (friendship == null)
            {
                return NotFound("好友請求不存在或已處理");
            }

            friendship.Status = "accepted";
            friendship.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("好友請求已接受");
        }

        // PUT: api/Friendship/5/reject
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectFriendRequest(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var friendship = await _context.Friendship
                .FirstOrDefaultAsync(f => f.Id == id && f.AddresseeId == userId && f.Status == "pending");

            if (friendship == null)
            {
                return NotFound("好友請求不存在或已處理");
            }

            friendship.Status = "rejected";
            friendship.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("好友請求已拒絕");
        }

        // DELETE: api/Friendship/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var friendship = await _context.Friendship
                .FirstOrDefaultAsync(f =>
                    f.Id == id &&
                    (f.RequesterId == userId || f.AddresseeId == userId));

            if (friendship == null)
            {
                return NotFound("好友關係不存在");
            }

            _context.Friendship.Remove(friendship);
            await _context.SaveChangesAsync();

            return Ok("好友關係已移除");
        }
    }
}