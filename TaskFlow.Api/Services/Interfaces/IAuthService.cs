using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services.Interfaces
{
    public interface IAuthService
    {
        // 1. Hàm Đăng ký: Nhận vào thông tin -> Trả về câu thông báo "Thành công/Thất bại"
        Task<string> RegisterAsync(RegisterRequestDto request);

        // 2. Hàm Đăng nhập: Nhận vào User/Pass -> Trả về cái Vé (LoginResponseDto)
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}