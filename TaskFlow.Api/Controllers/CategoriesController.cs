using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        // 1. Bây giờ Controller chỉ nói chuyện với Service (Quản lý), không gọi Repository hay DbContext nữa
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            // Service đã lo hết việc lấy dữ liệu và Map sang DTO rồi
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

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

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            // Service lo việc Map và Lưu
            var result = await _service.CreateAsync(request);

            // Trả về kết quả
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto request)
        {
            try
            {
                // Gọi Service để update
                var success = await _service.UpdateAsync(id, request);

                if (!success) return NotFound("Không tìm thấy Category để sửa");

                return NoContent();
            }
            catch (Exception ex)
            {
                // Bắt cái lỗi "Trùng tên" mà ta đã ném ra ở Service
                return BadRequest(ex.Message);
            }
        }

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
                // Hứng cái lỗi "vẫn còn công việc bên trong" và trả về 400 Bad Request
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}