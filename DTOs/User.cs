using System;
using System.Collections.Generic;
namespace AnimeNowApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required byte[] PasswordHash { get; set; }
        public required byte[] PasswordSalt { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<WatchHistory> WatchHistory { get; set; } = new List<WatchHistory>();
    }
}