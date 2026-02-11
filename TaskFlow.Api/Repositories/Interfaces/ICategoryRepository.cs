using TaskFlow.Api.Models;

namespace TaskFlow.Api.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(int userId); // Thêm userId
        Task<Category> GetByIdAsync(int id, int userId);     // Thêm userId
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> IsNameExistsAsync(string name, int excludeId, int userId); // Thêm userId
        Task<bool> HasTodoItemsAsync(int id); // Cái này giữ nguyên vì check theo CategoryId
    }
}
