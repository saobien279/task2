using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc phải có Token mới được vào
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            // 1. Móc UserId từ Token
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 2. Truyền xuống Service
            var result = await _service.GetAllAsync(userId);
            return Ok(result);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            // 1. Móc UserId
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 2. Truyền xuống Service
            var result = await _service.GetByIdAsync(id, userId);
            if (result == null)
            {
                return NotFound($"Không tìm thấy Category có ID = {id} hoặc bạn không có quyền xem!");
            }
            return Ok(result);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            // 1. Móc UserId
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 2. Truyền xuống để gán chủ sở hữu cho Category mới
            var result = await _service.CreateAsync(request, userId);
            
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto request)
        {
            try 
            {
                // 1. Móc UserId
                var userIdString = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int userId = int.Parse(userIdString);

                // 2. Truyền xuống Service
                var success = await _service.UpdateAsync(id, request, userId);
                
                if (!success) return NotFound("Không tìm thấy Category để sửa hoặc bạn không có quyền!");
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                // 1. Móc UserId
                var userIdString = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int userId = int.Parse(userIdString);

                // 2. Truyền xuống Service
                var success = await _service.DeleteAsync(id, userId);
                if (!success) return NotFound("Không tìm thấy danh mục để xóa hoặc bạn không có quyền!");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}