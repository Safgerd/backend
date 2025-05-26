using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using WebApp.Dao.DaoInterfaces;
using WebApp.Data.Models;
using WebApp.DTO;
using WebApp.Service.Interface;

namespace WebApp.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentDao _commentDao;
        private readonly IPostDao _postDao;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentService(ICommentDao commentDao, IPostDao postDao, UserManager<IdentityUser> userManager)
        {
            _commentDao = commentDao;
            _postDao = postDao;
            _userManager = userManager;
        }

        public async Task<CommentDto> GetCommentByIdAsync(int id)
        {
            var comment = await _commentDao.GetByIdAsync(id);
            return comment == null ? null : MapToCommentDto(comment);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsForPostAsync(int postId)
        {
            if (!await _postDao.ExistsAsync(postId))
            {
                return Enumerable.Empty<CommentDto>();
            }
            var comments = await _commentDao.GetByPostIdAsync(postId);
            return comments.Select(MapToCommentDto);
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto, string userId)
        {
            if (!await _postDao.ExistsAsync(createCommentDto.PostId))
            {
                return null;
            }

            var comment = new Comment
            {
                Text = createCommentDto.Text,
                PostId = createCommentDto.PostId,
                UserId = userId
            };
            var createdComment = await _commentDao.AddAsync(comment);
            createdComment.User = await _userManager.FindByIdAsync(userId);
            return MapToCommentDto(createdComment);
        }
        public async Task<bool> UpdateCommentAsync(int id, UpdateCommentDto updateDto, string userId)
        {
            var comment = await _commentDao.GetByIdAsync(id);
            if (comment == null) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;
            var userIsAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (comment.UserId != userId && !userIsAdmin)
            {
                return false;
            }

            comment.Text = updateDto.Text;
            await _commentDao.UpdateAsync(comment);
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id, string userId)
        {
            var comment = await _commentDao.GetByIdAsync(id);
            if (comment == null) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;
            var userIsAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (comment.UserId != userId && !userIsAdmin)
            {
                return false;
            }

            await _commentDao.DeleteAsync(id);
            return true;
        }

        // --- Mapper ---
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