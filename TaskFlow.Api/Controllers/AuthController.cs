using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Tiêm AuthService vào để sử dụng
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // API Đăng ký: POST /api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                // Trả về thông báo thành công
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                // Nếu lỗi (vd: Trùng tên), trả về lỗi 400 Bad Request
                return BadRequest(new { message = ex.Message });
            }
        }

        // API Đăng nhập: POST /api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                // Trả về Token
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Nếu sai mật khẩu, trả về lỗi 400
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}