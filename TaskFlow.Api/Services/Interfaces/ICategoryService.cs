using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        // Lấy danh sách đã Map sang DTO
        Task<IEnumerable<CategoriesResponseDto>> GetAllAsync();

        // Lấy 1 cái (trả về DTO, có thể null)
        Task<CategoriesResponseDto> GetByIdAsync(int id);

        // Tạo mới: Nhận vào RequestDTO, trả về ResponseDTO (để hiện ra cho người dùng)
        Task<CategoriesResponseDto> CreateAsync(CreateCategoryRequestDto request);

        // Cập nhật: Trả về bool (True = thành công, False = thất bại/không tìm thấy)
        Task<bool> UpdateAsync(int id, UpdateCategoryRequestDto request);

        // Xóa: Trả về bool
        Task<bool> DeleteAsync(int id);
    }
}
