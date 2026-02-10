using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        // Tiêm Service vào (thay vì Repository hay DbContext như trước)
        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound($"Không tìm thấy Category có ID = {id}");
            }
            return Ok(result);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            // Service sẽ lo việc Map và Lưu
            var result = await _service.CreateAsync(request);
            
            // Trả về 201 Created cùng với Location Header
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto request)
        {
            try 
            {
                // Service trả về True/False để biết update thành công hay không
                var success = await _service.UpdateAsync(id, request);
                
                if (!success) return NotFound("Không tìm thấy Category để sửa");
                
                return NoContent();
            }
            catch (Exception ex)
            {
                // Bắt lỗi Logic (ví dụ: Trùng tên) từ Service ném ra
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success) return NotFound("Không tìm thấy danh mục để xóa");

                return NoContent();
            }
            catch (Exception ex)
            {
                // QUAN TRỌNG: Bắt lỗi "Vẫn còn TodoItem bên trong" để báo cho user
                // Trả về 400 Bad Request kèm lời nhắn
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}