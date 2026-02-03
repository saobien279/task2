using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
