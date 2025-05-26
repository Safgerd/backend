using Microsoft.EntityFrameworkCore;
using WebApp.Dao.DaoInterfaces;
using WebApp.Data;
using WebApp.Data.Models;

namespace WebApp.Dao
{
    public class PostDao : IPostDao
    {
        private readonly ApplicationDbContext _context;

        public PostDao(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Skip(pageSize * pageNumber)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Posts.CountAsync();
        }


        public async Task<Post> AddAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task UpdateAsync(Post post)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            if (existingPost != null)
            {
                existingPost.Title = post.Title;
                existingPost.Content = post.Content;
                _context.Entry(existingPost).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Posts.AnyAsync(e => e.Id == id);
        }
    }
}
