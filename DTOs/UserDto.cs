using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace AnimeNowApi.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? Token { get; set; } // Token 可以保持可空
    }

    public class UserLoginResponseDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Token { get; set; }
    }

    public class RegisterDto
    {
        [JsonProperty("username")]
        [Required(ErrorMessage = "用戶名是必須項")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "用戶名長度必須在3-50個字之間")]
        public required string Username { get; set; }

        [JsonProperty("email")]
        [Required(ErrorMessage = "郵箱是必須項")]
        [EmailAddress(ErrorMessage = "郵箱格式不正確")]
        public required string Email { get; set; }

        [JsonProperty("password")]
        [Required(ErrorMessage = "密碼是必須項")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度必須在3-50個字之間")]
        public required string Password { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}