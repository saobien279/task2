using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.DTOs;
namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TodoItemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<IActionResult> GetToDoItems()
        {
            var todoItems = await _context.TodoItems
                                  .Include(t => t.Category)
                                  .ToListAsync();

        // Chuyển đổi sang List DTO


        var result = todoItems.Select(t => {
        // Kiểm tra xem đối tượng Category có tồn tại hay không
        string name;
        if (t.Category != null) 
        {
            name = t.Category.Name; // Nếu có thì lấy tên
        }
        else 
        {
            name = "N/A"; // Nếu không có thì gán là N/A
        }

        return new TodoItemResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            IsCompleted = t.IsCompleted,
            CategoryId = t.CategoryId,
            CategoryName = name
        };
        }).ToList();

        return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> CreateToDoItem([FromBody] CreateTodoItemRequestDto request)
        {
            //Kiem tra du lieu loi
            if (request == null)
            {
                return BadRequest("Dữ liệu không được để trống");
            }

            //mapping chuyen doi
            var todoItems = new TodoItem
            {
                Title = request.Title,
                IsCompleted = request.IsCompleted,
                CategoryId = request.CategoryId,
            };

            //luu vao db
            _context.TodoItems.Add(todoItems);
            await _context.SaveChangesAsync();

            await _context.Entry(todoItems).Reference(t => t.Category).LoadAsync();
            //reponse to user
            var responseDto = new TodoItemResponseDto
            {
                Id = todoItems.Id,
                Title = todoItems.Title,
                IsCompleted = todoItems.IsCompleted,
                CategoryId = todoItems.CategoryId,
                CategoryName = todoItems.Category?.Name ?? "N/A"
            };
            return CreatedAtAction(nameof(GetToDoItems), new { id = responseDto.Id }, responseDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, [FromBody] TodoItem request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID không khớp!");
            }
            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TodoItems.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            // 1. Tìm xem cái món cần xóa có tồn tại không
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound("Không tìm thấy công việc này để xóa!");
            }

            // 2. Ra lệnh cho EF Core: "Gỡ thằng này ra khỏi bảng"
            _context.TodoItems.Remove(todoItem);

            // 3. Chốt đơn xuống Database
            await _context.SaveChangesAsync();

            // 4. Trả về 204 hoặc 200 kèm thông báo
            return NoContent();
        }
    }
}



