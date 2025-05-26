using WebApp.DTO;

namespace WebApp.Service.Interface
{
    public interface ICommentService
    {
        Task<CommentDto> GetCommentByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetCommentsForPostAsync(int postId);
        Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto, string userId);
        Task<bool> UpdateCommentAsync(int id, UpdateCommentDto updateDto, string userId);
        Task<bool> DeleteCommentAsync(int id, string userId);
    }
}
