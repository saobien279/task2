using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
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
            var list = await _context.TodoItems.ToListAsync();
            return Ok(list);
        }

        [HttpPost]

        public async Task<IActionResult> CreateToDoItem([FromBody] TodoItem request)
        {
            if (request == null)
            {
                return BadRequest("Khong Duoc De Trong");
            }

            request.Id = 0;

            _context.TodoItems.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetToDoItems), new { id = request.Id }, request);
        }
    }
}
