using TaskFlow.Api.Models;

namespace TaskFlow.Api.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        // 1. Hàm lấy danh sách (kèm userId)
        Task<IEnumerable<Category>> GetAllAsync(int userId);

        // 2. Hàm cũ (để TodoItem gọi - KHÔNG ĐƯỢC XÓA)
        //Task<Category> GetByIdAsync(int id);

        // 3. Hàm mới (để CategoryService gọi - QUAN TRỌNG NHẤT LÚC NÀY)
        Task<Category> GetByIdAsync(int id, int userId);

        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);

        // 4. Hàm check tên (kèm userId)
        Task<bool> ExistsAsync(int id);
        Task<bool> IsNameExistsAsync(string name, int excludeId, int userId);

        Task<bool> HasTodoItemsAsync(int id);
    }
}