using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.DTOs;
using AutoMapper;
namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TodoItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> GetToDoItems()
        {
            var todoItems = await _context.TodoItems
                                  .Include(t => t.Category)
                                  .ToListAsync();

            var result = _mapper.Map<List<TodoItemResponseDto>>(todoItems);
            return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> CreateToDoItem([FromBody] CreateTodoItemRequestDto request)
        {
            //mapping chuyen doi
            var todoItem = _mapper.Map<TodoItem>(request);

            //luu vao db
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            await _context.Entry(todoItem).Reference(t => t.Category).LoadAsync();
            //reponse to user
            var responseDto = _mapper.Map<TodoItemResponseDto>(todoItem);
            return CreatedAtAction(nameof(GetToDoItems), new { id = responseDto.Id }, responseDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, [FromBody] UpdateTodoItemRequestDto request)
        {


            // 2. Tìm món đồ cũ trong kho (Database)
            var todoItem = await _context.TodoItems.FindAsync(id);

            // 3. Nếu không tìm thấy -> Báo lỗi 404
            if (todoItem == null)
            {
                return NotFound($"Không tìm thấy công việc có ID = {id}");
            }

            // 4. Kiểm tra xem CategoryId mới có tồn tại không (Optional nhưng nên làm)
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category ID không tồn tại!");
            }

            // 5. Mapping: Đổ dữ liệu MỚI đè lên dữ liệu CŨ
            _mapper.Map(request, todoItem);

            // 6. Lưu thay đổi
            await _context.SaveChangesAsync();

            // 7. Trả về 204 No Content (Thành công nhưng không cần trả dữ liệu gì)
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



