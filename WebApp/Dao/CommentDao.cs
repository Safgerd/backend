using Microsoft.EntityFrameworkCore;
using WebApp.Dao.DaoInterfaces;
using WebApp.Data;
using WebApp.Data.Models;

namespace WebApp.Dao
{
    public class CommentDao : ICommentDao
    {
        private readonly ApplicationDbContext _context;

        public CommentDao(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task UpdateAsync(Comment comment)
        {
            var existingComment = await _context.Comments.FindAsync(comment.Id);
            if (existingComment != null)
            {
                existingComment.Text = comment.Text;
                _context.Entry(existingComment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
