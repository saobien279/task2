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

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // Logic cũ của bạn: Lấy list kèm theo TodoItems để đếm
            return await _context.Categories
                                 .Include(c => c.TodoItems)
                                 .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
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

        public async Task<bool> IsNameExistsAsync(string name, int excludeId)
        {
            // Kiểm tra trùng tên nhưng trừ chính nó ra (dùng cho Update)
            return await _context.Categories.AnyAsync(c => c.Name == name && c.Id != excludeId);
        }

        public async Task<bool> HasTodoItemsAsync(int categoryId)
        {
            return await _context.TodoItems.AnyAsync(t => t.CategoryId == categoryId);
        }
    }
}
