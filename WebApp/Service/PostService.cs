using Microsoft.AspNetCore.Identity;
using WebApp.Dao.DaoInterfaces;
using WebApp.Data.Models;
using WebApp.DTO;
using WebApp.Service.Interface;

namespace WebApp.Service
{
    public class PostService : IPostService
    {
        private readonly IPostDao _postDao;
        private readonly ICommentDao _commentDao;
        private readonly UserManager<IdentityUser> _userManager;

        public PostService(IPostDao postDao, ICommentDao commentDao, UserManager<IdentityUser> userManager)
        {
            _postDao = postDao;
            _commentDao = commentDao;
            _userManager = userManager;
        }

        public async Task<PostDto> GetPostByIdAsync(int id)
        {
            var post = await _postDao.GetByIdAsync(id);
            return post == null ? null : MapToPostDto(post);
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var posts = await _postDao.GetAllAsync();
            return posts.Select(MapToPostDto);
        }

        public async Task<(IEnumerable<PostDto> Posts, int TotalPages)> GetPaginatedPostsAsync(int pageNumber, int pageSize)
        {
            var posts = await _postDao.GetPaginatedAsync(pageNumber, pageSize);
            var totalPosts = await _postDao.GetCountAsync();
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            return (posts.Select(MapToPostDto), totalPages);
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, string userId)
        {
            var post = new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                UserId = userId
            };
            var createdPost = await _postDao.AddAsync(post);
            createdPost.User = await _userManager.FindByIdAsync(userId);
            return MapToPostDto(createdPost);
        }

        public async Task<bool> UpdatePostAsync(int id, UpdatePostDto updatePostDto, string userId)
        {
            var post = await _postDao.GetByIdAsync(id);
            if (post == null) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;
            var userIsAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (post.UserId != userId && !userIsAdmin)
            {
                return false;
            }

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            await _postDao.UpdateAsync(post);
            return true;
        }

        public async Task<bool> DeletePostAsync(int id, string userId)
        {
            var post = await _postDao.GetByIdAsync(id);
            if (post == null) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;
            var userIsAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (post.UserId != userId && !userIsAdmin)
            {
                return false;
            }

            await _postDao.DeleteAsync(id);
            return true;
        }

        private PostDto MapToPostDto(Post post)
        {
            if (post == null) return null;
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                UserName = post.User?.Email,
                Comments = post.Comments?.Select(MapToCommentDto).ToList() ?? new List<CommentDto>()
            };
        }

        private CommentDto MapToCommentDto(Comment comment)
        {
            if (comment == null) return null;
            return new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                UserId = comment.UserId,
                UserName = comment.User?.Email,
                PostId = comment.PostId
            };
        }
    }
}
