using WebApp.Data.Models;

namespace WebApp.Dao.DaoInterfaces
{
    public interface ICommentDao
    {
        Task<Comment> GetByIdAsync(int id);
        Task<IEnumerable<Comment>> GetByPostIdAsync(int postId);
        Task<Comment> AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
    }
}
