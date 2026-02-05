using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải dài ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;
        // Lưu ý: Đây là Password thô (vd: "123456"), không phải Hash

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
