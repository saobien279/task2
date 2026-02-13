using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _service;

        public TodoItemsController(ITodoItemService service)
        {
            _service = service;
        }

        // Helper function để lấy UserId cho gọn code
        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin User.");
            }
            return int.Parse(userIdString);
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoItems([FromQuery] TodoItemParameters parameters)
        {
            try
            {
                int userId = GetCurrentUserId();
                var result = await _service.GetAllAsync(parameters, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItemById(int id)
        {
            try
            {
                int userId = GetCurrentUserId();
                var result = await _service.GetByIdAsync(id, userId);

                if (result == null)
                {
                    return NotFound($"Không tìm thấy công việc ID = {id} hoặc bạn không có quyền xem.");
                }
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodoItem([FromBody] CreateTodoItemRequestDto request)
        {
            try
            {
                int userId = GetCurrentUserId();
                var result = await _service.CreateAsync(request, userId);

                return CreatedAtAction(nameof(GetTodoItemById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, [FromBody] UpdateTodoItemRequestDto request)
        {
            try
            {
                int userId = GetCurrentUserId();
                var success = await _service.UpdateAsync(id, request, userId);

                if (!success) return NotFound($"Không tìm thấy công việc ID = {id} hoặc bạn không có quyền sửa.");

                return NoContent();
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            try
            {
                int userId = GetCurrentUserId();
                var success = await _service.DeleteAsync(id, userId);

                if (!success) return NotFound($"Không tìm thấy công việc ID = {id} hoặc bạn không có quyền xóa.");

                return NoContent();
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
        }
    }
}