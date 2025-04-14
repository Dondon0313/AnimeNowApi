using System;

namespace AnimeNowApi.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int TargetId { get; set; }
        public required string TargetType { get; set; }
        public required string Content { get; set; }
        public DateTime Created { get; set; }
        public required string Username { get; set; }
    }

    public class CreateCommentDto
    {
        public int TargetId { get; set; }
        public required string TargetType { get; set; }
        public required string Content { get; set; }
    }
}