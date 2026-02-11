using AutoMapper;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;
using TaskFlow.Api.Repositories.Interfaces;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoriesResponseDto>> GetAllAsync(int userId)
        {
            // 1. Truyền userId xuống Repo để lọc lấy đúng đồ của mình
            var categories = await _repository.GetAllAsync(userId);
            return _mapper.Map<IEnumerable<CategoriesResponseDto>>(categories);
        }

        public async Task<CategoriesResponseDto> GetByIdAsync(int id, int userId)
        {
            // Tìm đúng ID và phải đúng chủ sở hữu
            var category = await _repository.GetByIdAsync(id, userId);
            if (category == null) return null;

            return _mapper.Map<CategoriesResponseDto>(category);
        }

        public async Task<CategoriesResponseDto> CreateAsync(CreateCategoryRequestDto request, int userId)
        {
            var category = _mapper.Map<Category>(request);

            // --- QUAN TRỌNG NHẤT: ĐÁNH DẤU CHỦ SỞ HỮU ---
            category.UserId = userId;

            await _repository.AddAsync(category);
            return _mapper.Map<CategoriesResponseDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequestDto request, int userId)
        {
            // 1. Tìm bản ghi (kèm userId để đảm bảo không sửa bậy đồ của người khác)
            var category = await _repository.GetByIdAsync(id, userId);
            if (category == null) return false;

            // 2. Kiểm tra trùng tên (Trong phạm vi các Category của user này)
            if (request.Name != null)
            {
                bool isDuplicate = await _repository.IsNameExistsAsync(request.Name, id, userId);
                if (isDuplicate)
                {
                    throw new Exception("Tên danh mục này đã tồn tại!");
                }
            }

            // 3. Map đè và lưu
            _mapper.Map(request, category);
            await _repository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            // 1. Tìm bản ghi (kèm userId để đảm bảo không xóa bậy đồ người khác)
            var category = await _repository.GetByIdAsync(id, userId);
            if (category == null) return false;

            // 2. Check xem có công việc nào bên trong không
            bool hasItems = await _repository.HasTodoItemsAsync(id);
            if (hasItems)
            {
                throw new Exception("Không thể xóa danh mục này vì vẫn còn công việc bên trong!");
            }

            // 3. Xóa
            await _repository.DeleteAsync(category);
            return true;
        }
    }
}