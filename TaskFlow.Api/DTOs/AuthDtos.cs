using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;
        // Lưu ý: Đây là Password thô (vd: "123456"), không phải Hash

        [Required(ErrorMessage = "Vai trò (Role) không được để trống")]
        [RegularExpression("^(Admin|User)$", ErrorMessage = "Role chỉ được là 'Admin' hoặc 'User'")]
        public string Role { get; set; } = "User"; // Mặc định là User thường
    }

    // 2. Cái giỏ dùng cho Đăng Nhập
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // 3. Cái giỏ dùng để trả về kết quả (Vé vào cửa)
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty; // Chuỗi JWT dài ngoằng
    }
}
