using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.Repositories.Interfaces;

namespace TaskFlow.Api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int userId)
        {
            // Lọc chỉ lấy Category của user đang đăng nhập
                return await _context.Categories
                    .Where(c => c.UserId == userId)
                    .Include(t => t.TodoItems)
                    .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id, int userId)
        {
            // Tìm đúng ID và phải đúng chủ sở hữu
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }


        //kiemtra
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> IsNameExistsAsync(string name, int excludeId, int userId)
        {
            // Kiểm tra trùng tên nhưng chỉ trong phạm vi các Category của user này
            return await _context.Categories
                .AnyAsync(c => c.Name == name && c.Id != excludeId && c.UserId == userId);
        }

        public async Task<bool> HasTodoItemsAsync(int categoryId)
        {
            return await _context.TodoItems.AnyAsync(t => t.CategoryId == categoryId);
        }
    }
}
