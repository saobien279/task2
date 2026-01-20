using AutoMapper;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;
using TaskFlow.Api.Repositories.Interfaces;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository; // Gọi Thủ kho
        private readonly IMapper _mapper;                 // Gọi Máy đóng gói

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CategoriesResponseDto>> GetAllAsync()
        {
            // 1. Gọi Repo lấy dữ liệu thô
            var categories = await _repository.GetAllAsync();

            // 2. Map sang DTO
            return _mapper.Map<IEnumerable<CategoriesResponseDto>>(categories);
        }

        public async Task<CategoriesResponseDto> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;

            return _mapper.Map<CategoriesResponseDto>(category);
        }

        public async Task<CategoriesResponseDto> CreateAsync(CreateCategoryRequestDto request)
        {
            // 1. Map từ DTO sang Entity
            var category = _mapper.Map<Category>(request);

            // 2. Gọi Repo lưu xuống DB
            await _repository.AddAsync(category);

            // 3. Map ngược lại sang ResponseDTO (đã có ID mới sinh) để trả về
            return _mapper.Map<CategoriesResponseDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequestDto request)
        {
            // 1. Tìm bản ghi cũ
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            // 2. CHECK LOGIC: Kiểm tra trùng tên (Nghiệp vụ nằm ở đây!)
            // Nếu tên mới khác null VÀ tên đó đã tồn tại ở bản ghi khác
            if (request.Name != null)
            {
                bool isDuplicate = await _repository.IsNameExistsAsync(request.Name, id);
                if (isDuplicate)
                {
                    throw new Exception("Tên danh mục này đã tồn tại!");
                    // (Tạm thời ném lỗi, sau này ta sẽ có cách xử lý mượt hơn)
                }
            }

            // 3. Map đè dữ liệu mới lên cũ
            _mapper.Map(request, category);

            // 4. Lưu
            await _repository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // 1. Kiểm tra xem Category có tồn tại không
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            // 2. LOGIC MỚI: Kiểm tra xem có việc nào bên trong không
            bool hasItems = await _repository.HasTodoItemsAsync(id);
            if (hasItems)
            {
                // Ném ra một ngoại lệ để báo cho Controller biết lý do
                throw new Exception("Không thể xóa danh mục này vì vẫn còn công việc bên trong!");
            }

            // 3. Nếu trống thì mới cho xóa
            await _repository.DeleteAsync(category);
            return true;
        }
    }
}
