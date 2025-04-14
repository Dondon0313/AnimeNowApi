namespace AnimeNowApi.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int AddresseeId { get; set; }
        public required string Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public required User Requester { get; set; }
        public required User Addressee { get; set; }
    }
}