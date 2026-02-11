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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CẤU HÌNH MỐI QUAN HỆ ---

            // 1. Một User có nhiều TodoItems
            modelBuilder.Entity<TodoItem>()
                .HasOne<User>() // Một TodoItem thuộc về một User
                .WithMany()     // Một User có nhiều TodoItem (nhưng ta ko cần khai báo List<TodoItem> bên User nên để trống)
                .HasForeignKey(t => t.UserId) // Khóa ngoại là UserId
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Một User có nhiều Categories
            modelBuilder.Entity<Category>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa User -> Giữ lại Category (hoặc Cascade tùy bạn)
        }
    }
}
