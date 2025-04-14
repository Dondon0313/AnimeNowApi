// FriendshipDto.cs
namespace AnimeNowApi.DTOs
{
    public class FriendshipDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FriendRequestDto
    {
        public int AddresseeId { get; set; }
    }
}