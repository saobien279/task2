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

        public async Task<(IEnumerable<TodoItem>, int)> GetAllAsync(TodoItemParameters parameters)
        {
            // BƯỚC 1: Khởi tạo truy vấn (Chưa chạy xuống DB)
            // .AsQueryable() nghĩa là: "Khoan đã, đừng lấy vội, để tôi lắp ghép điều kiện đã"
            var query = _context.TodoItems.Include(t => t.Category).AsQueryable();

            // BƯỚC 2: Lắp ghép điều kiện Lọc (Filtering)
            if (parameters.CategoryId.HasValue)
            {
                // Thêm câu "WHERE CategoryId = ..." vào lệnh SQL
                query = query.Where(t => t.CategoryId == parameters.CategoryId.Value);
            }

            // BƯỚC 3: Lắp ghép điều kiện Tìm kiếm (Searching)
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                // Thêm câu "WHERE Title LIKE %...%" vào lệnh SQL
                query = query.Where(t => t.Title.Contains(parameters.SearchTerm));
            }

            // --- CHÈN CODE MỚI VÀO ĐÂY (BƯỚC 3.5: SẮP XẾP) ---

            // Kiểm tra xem người dùng muốn sắp xếp theo cái gì?
            switch (parameters.SortBy.ToLower()) // Chuyển về chữ thường cho dễ so sánh
            {
                case "title":
                    // Nếu chọn "title": Kiểm tra xem muốn Tăng hay Giảm
                    if (parameters.IsDescending == true)
                    {
                        // Giảm dần (Z -> A)
                        query = query.OrderByDescending(t => t.Title);
                    }
                    else
                    {
                        // Tăng dần (A -> Z)
                        query = query.OrderBy(t => t.Title);
                    }
                    break;

                default:
                    // Mặc định (hoặc nhập lung tung): Sắp xếp theo ID
                    // Mặc định (hoặc nhập lung tung): Xếp theo ID
                    // Đây chính là case "id" bạn nói
                    if (parameters.IsDescending == true)
                    {
                        // ID Giảm dần (Mới nhất lên đầu)
                        query = query.OrderByDescending(t => t.Id);
                    }
                    else
                    {
                        // ID Tăng dần (Cũ nhất lên đầu)
                        query = query.OrderBy(t => t.Id);
                    }
                    break;
            }

            // BƯỚC 4: Đếm tổng số lượng (Quan trọng!)
            // Phải đếm TRƯỚC khi phân trang để biết có tất cả bao nhiêu kết quả thỏa mãn điều kiện tìm kiếm
            int totalCount = await query.CountAsync();

            // BƯỚC 5: Phân trang (Paging)
            // Công thức: Bỏ qua (Trang hiện tại - 1) * Kích thước, sau đó Lấy (Kích thước) dòng
            // Ví dụ: Trang 2, mỗi trang 10 dòng -> Bỏ qua 10 dòng đầu, lấy 10 dòng tiếp theo (dòng 11-20)
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(); // <--- Lúc này lệnh SQL mới thực sự chạy xuống Database!

            // Trả về cả 2 thứ: Danh sách đã phân trang VÀ Tổng số lượng
            return (items, totalCount);
        }

        public async Task<TodoItem> GetByIdAsync(int id)
        {
            // SỬA LẠI: FindAsync không dùng được Include.
            // Phải dùng FirstOrDefaultAsync để nạp thông tin Category đi kèm.
            return await _context.TodoItems
                                 .Include(t => t.Category)
                                 .FirstOrDefaultAsync(t => t.Id == id);
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