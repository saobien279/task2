using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.DTOs;
using System.ComponentModel.DataAnnotations;


namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var list = await _context.Categories
                             .Include(c => c.TodoItems) 
                             .ToListAsync();
            var result = list.Select(c => new CategoriesResponseDto 
            {
                Id = c.Id,
                Name = c.Name,
                TotalItem = c.TodoItems.Count()
            }).ToList();

            return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Khong Duoc De Trong");
            }

            var category = new Category()
            {
                Name = request.Name,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var responDto = new CategoriesResponseDto
            {
                Id = category.Id,
                Name = request.Name,
                TotalItem = 0
            };
            return CreatedAtAction(
            nameof(GetCategories),
            new { id = responDto.Id },
            responDto);
        }
    }
}
