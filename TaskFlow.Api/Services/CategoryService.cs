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
            // Lỗi sẽ biến mất nếu bạn đã sửa ICategoryRepository
            var categories = await _repository.GetAllAsync(userId);
            return _mapper.Map<IEnumerable<CategoriesResponseDto>>(categories);
        }

        public async Task<CategoriesResponseDto> GetByIdAsync(int id, int userId)
        {
            // Tìm Category theo đúng ID và UserId của người đó
            var category = await _repository.GetByIdAsync(id, userId);
            if (category == null) return null;

            return _mapper.Map<CategoriesResponseDto>(category);
        }

        public async Task<CategoriesResponseDto> CreateAsync(CreateCategoryRequestDto request, int userId)
        {
            var category = _mapper.Map<Category>(request);

            // Gán UserId vào để đánh dấu chủ quyền
            category.UserId = userId;

            await _repository.AddAsync(category);
            return _mapper.Map<CategoriesResponseDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequestDto request, int userId)
        {
            // 1. Kiểm tra quyền sở hữu trước khi sửa
            var category = await _repository.GetByIdAsync(id, userId);
            if (category == null) return false;

            // 2. Check trùng tên (trong phạm vi UserId đó)
            if (request.Name != null)
            {
                bool isDuplicate = await _repository.IsNameExistsAsync(request.Name, id, userId);
                if (isDuplicate) throw new Exception("Tên danh mục này đã tồn tại!");
            }

            _mapper.Map(request, category);
            await _repository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            // 1. Kiểm tra quyền sở hữu trước khi xóa
            var category = await _repository.GetByIdAsync(id, userId);
            if (category == null) return false;

            // 2. Check ràng buộc dữ liệu
            bool hasItems = await _repository.HasTodoItemsAsync(id);
            if (hasItems) throw new Exception("Không thể xóa danh mục này vì vẫn còn công việc bên trong!");

            await _repository.DeleteAsync(category);
            return true;
        }
    }
}