using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        // Lưu ý: Đây là PasswordHash (đã băm), KHÔNG lưu password thường
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Vai trò: "Admin" hoặc "User". Mặc định là "User"
        public string Role { get; set; } = "User";
    }
}
