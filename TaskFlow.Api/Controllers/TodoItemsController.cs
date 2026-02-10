using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _service;

        // Tiêm Service vào (thay vì DbContext & Mapper)
        public TodoItemsController(ITodoItemService service)
        {
            _service = service;
        }

        // GET: api/TodoItems
        // 1. Thêm tham số [FromQuery] để lấy dữ liệu từ URL (vd: ?pageNumber=1&searchTerm=Hoc)
        [HttpGet]
        public async Task<IActionResult> GetTodoItems([FromQuery] TodoItemParameters parameters)
        {
            // 2. Gọi Service (Service đã trả về PagedResult xịn xò)
            var result = await _service.GetAllAsync(parameters);

            // 3. Trả về kết quả
            return Ok(result);
        }

        // GET: api/TodoItems/5
        // (Tôi thêm hàm này để CreatedAtAction bên dưới có chỗ trỏ về)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItemById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound($"Không tìm thấy công việc ID = {id}");
            }
            return Ok(result);
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<IActionResult> CreateTodoItem([FromBody] CreateTodoItemRequestDto request)
        {
            try
            {
                var result = await _service.CreateAsync(request);

                // Trả về 201 Created
                return CreatedAtAction(nameof(GetTodoItemById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                // Bắt lỗi "Category không tồn tại" từ Service ném ra
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, [FromBody] UpdateTodoItemRequestDto request)
        {
            try
            {
                // Lưu ý: Không cần check id != request.Id nữa nếu DTO không có Id
                var success = await _service.UpdateAsync(id, request);

                if (!success) return NotFound($"Không tìm thấy công việc ID = {id} để sửa");

                return NoContent();
            }
            catch (Exception ex)
            {
                // Bắt lỗi logic (ví dụ: đổi sang Category không tồn tại)
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound($"Không tìm thấy công việc ID = {id} để xóa");

            return NoContent();
        }
    }
}