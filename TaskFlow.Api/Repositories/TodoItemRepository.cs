using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Repositories.Interfaces;

namespace TaskFlow.Api.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly AppDbContext _context;

        public TodoItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<TodoItem>, int)> GetAllAsync(TodoItemParameters parameters, int userId)
        {
            // BƯỚC 1: Khởi tạo truy vấn
            var query = _context.TodoItems.Include(t => t.Category).AsQueryable();

            // --- QUAN TRỌNG: LỌC THEO USER ID ---
            query = query.Where(t => t.UserId == userId);
            // ------------------------------------

            // BƯỚC 2: Lọc theo Category (nếu có)
            if (parameters.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == parameters.CategoryId.Value);
            }

            // BƯỚC 3: Tìm kiếm
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query.Where(t => t.Title.Contains(parameters.SearchTerm));
            }

            // BƯỚC 3.5: Sắp xếp
            switch (parameters.SortBy.ToLower())
            {
                case "title":
                    if (parameters.IsDescending == true)
                        query = query.OrderByDescending(t => t.Title);
                    else
                        query = query.OrderBy(t => t.Title);
                    break;

                default:
                    if (parameters.IsDescending == true)
                        query = query.OrderByDescending(t => t.Id);
                    else
                        query = query.OrderBy(t => t.Id);
                    break;
            }

            // BƯỚC 4: Đếm tổng số lượng (Đã lọc theo User)
            int totalCount = await query.CountAsync();

            // BƯỚC 5: Phân trang
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<TodoItem> GetByIdAsync(int id, int userId)
        {
            // SỬA LẠI: FindAsync không dùng được Include.
            // Phải dùng FirstOrDefaultAsync để nạp thông tin Category đi kèm.
            return await _context.TodoItems
                                 .Include(t => t.Category)
                                 .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task AddAsync(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoItem todoItem)
        {
            _context.TodoItems.Update(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodoItem todoItem)
        {
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
        }
    }
}