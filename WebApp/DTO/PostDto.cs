using WebApp.Data.Models;

namespace WebApp.DTO
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }

    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class UpdatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
