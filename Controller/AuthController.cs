using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using AnimeNowApi.Models;
using AnimeNowApi.DTOs;
using AnimeNowApi.Services;
using AnimeNowApi.Data;
using AutoMapper;
using System.Security.Claims;
namespace AnimeNowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AuthController(AnimeDbContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // 檢查用戶名和郵箱是否已存在
            if (await UserExists(registerDto.Username, registerDto.Email))
                return BadRequest("用戶名或郵箱已被使用");

            // 創建密碼
            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("註冊成功");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserLoginResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Username == loginDto.Username);

            if (user == null) return Unauthorized("用戶名或密碼不正確");

            if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("用戶名或密碼不正確");

            return new UserLoginResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            };
        }

       
        private async Task<bool> UserExists(string username, string email)
        {
            return await _context.Users.AnyAsync(x =>
                x.Username.ToLower() == username.ToLower() ||
                x.Email.ToLower() == email.ToLower());
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}