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

        // Thực thi hàm GetAll có userId
        public async Task<IEnumerable<Category>> GetAllAsync(int userId)
        {
            return await _context.Categories
                .Include(c => c.TodoItems)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        // Thực thi hàm cũ (cho TodoItem)
        //public async Task<Category> GetByIdAsync(int id)
        //{
        //    return await _context.Categories
        //        .FirstOrDefaultAsync(c => c.Id == id);
        //}

        // Thực thi hàm mới (cho CategoryService) -> HẾT LỖI Ở ĐÂY
        public async Task<Category> GetByIdAsync(int id, int userId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        // Thực thi hàm check tên có userId
        public async Task<bool> IsNameExistsAsync(string name, int excludeId, int userId)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name == name && c.Id != excludeId && c.UserId == userId);
        }

        public async Task<bool> HasTodoItemsAsync(int id)
        {
            return await _context.TodoItems.AnyAsync(t => t.CategoryId == id);
        }
    }
}