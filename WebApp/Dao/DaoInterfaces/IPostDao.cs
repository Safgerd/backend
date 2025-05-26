using WebApp.Data.Models;

namespace WebApp.Dao.DaoInterfaces
{
    public interface IPostDao
    {
        Task<Post> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task<IEnumerable<Post>> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<int> GetCountAsync();
        Task<Post> AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
