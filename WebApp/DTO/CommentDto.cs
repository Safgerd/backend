namespace WebApp.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } // Для отображения имени автора
        public int PostId { get; set; }
    }

    public class CreateCommentDto
    {
        public string Text { get; set; }
        public int PostId { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Text { get; set; }
    }
}
