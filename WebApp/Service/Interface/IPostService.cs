using WebApp.DTO;

namespace WebApp.Service.Interface
{
    public interface IPostService
    {
        Task<PostDto> GetPostByIdAsync(int id);
        Task<IEnumerable<PostDto>> GetAllPostsAsync();
        Task<(IEnumerable<PostDto> Posts, int TotalPages)> GetPaginatedPostsAsync(int pageNumber, int pageSize);
        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, string userId);
        Task<bool> UpdatePostAsync(int id, UpdatePostDto updatePostDto, string userId);
        Task<bool> DeletePostAsync(int id, string userId);
    }
}
