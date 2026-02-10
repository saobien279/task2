using System.IdentityModel.Tokens.Jwt; // Thư viện để tạo Token
using System.Security.Claims;          // Thư viện để tạo các "Lời tuyên bố" (Claims)
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;  // Thư viện mã hóa Token
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; // Dùng để đọc file appsettings.json (lấy khóa bí mật)

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- A. XỬ LÝ ĐĂNG KÝ ---
        public async Task<string> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Kiểm tra xem Username đã có người dùng chưa
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                throw new Exception("Tên đăng nhập đã tồn tại!");
            }

            // 2. BĂM MẬT KHẨU (Hashing)
            // Đây là bước quan trọng nhất! Biến "123456" thành "$2a$11$..."
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo User mới để lưu vào DB
            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash, // Lưu cái đã băm
                Role = request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Đăng ký thành công!";
        }

        // --- B. XỬ LÝ ĐĂNG NHẬP ---
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // 1. Tìm User trong Database theo Username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. Kiểm tra 2 điều kiện:
            //    - User có tồn tại không?
            //    - Mật khẩu nhập vào (request.Password) khi băm ra có khớp với cái trong DB không?
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Sai tên đăng nhập hoặc mật khẩu!");
            }

            // 3. Nếu đúng hết -> Tiến hành tạo Vé (Token)
            string token = CreateToken(user);

            // 4. Trả vé cho người dùng
            return new LoginResponseDto { Token = token };
        }

        // --- C. HÀM PHỤ: TẠO TOKEN (IN VÉ) ---
        // Hàm này private vì chỉ dùng nội bộ trong class này
        private string CreateToken(User user)
        {
            // 1. Tạo các "Claim" (Thông tin được in lên vé)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),       // Tên user
                new Claim(ClaimTypes.Role, user.Role),           // Quyền (Admin/User)
                new Claim("UserId", user.Id.ToString())          // ID riêng (để sau này biết ai đang gọi API)
            };

            // 2. Lấy "Chìa khóa bí mật" từ file cấu hình (appsettings.json)
            // Lưu ý: Đoạn này tí nữa mình phải thêm vào file json
            var keyString = _configuration.GetSection("AppSettings:Token").Value;
            if (string.IsNullOrEmpty(keyString))
            {
                throw new Exception("Chưa cấu hình khóa bí mật trong appsettings.json!");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));//thanh 1 chuoi nhi phan

            // 3. Tạo chữ ký số (Dùng thuật toán HmacSha256)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // 4. Quy định thông tin của Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // Vé hết hạn sau 1 ngày
                SigningCredentials = creds
            };

            // 5. Viết Token ra thành chuỗi
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); // Trả về chuỗi: "eyJhbGci..."
        }
    }
}