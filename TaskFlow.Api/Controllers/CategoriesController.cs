using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;


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
            var list = await _context.Categories.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category request)
        {
            if (request == null)
            {
                return BadRequest("Khong Duoc De Trong");
            }

            request.Id = 0;

            _context.Categories.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
            nameof(GetCategories),
            new { id = request.Id },
            request);
        }
    }
}
