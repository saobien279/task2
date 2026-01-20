using TaskFlow.Api.Models;

namespace TaskFlow.Api.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);

        Task AddAsync(Category category);
        Task UpdateAsync(Category category); // Chỉ update xuống DB
        Task DeleteAsync(Category category); // Xóa đối tượng đã tìm thấy

        Task<bool> ExistsAsync(int id);
        Task<bool> IsNameExistsAsync(string name, int excludeId);

        // Thêm vào Interface
        Task<bool> HasTodoItemsAsync(int categoryId);
    }
}
