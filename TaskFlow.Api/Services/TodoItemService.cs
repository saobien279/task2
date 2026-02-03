using AutoMapper;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;
using TaskFlow.Api.Repositories.Interfaces;
using TaskFlow.Api.Services.Interfaces;

namespace TaskFlow.Api.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ITodoItemRepository _repository;
        private readonly ICategoryRepository _categoryRepository; // Gọi thêm ông hàng xóm
        private readonly IMapper _mapper;

        public TodoItemService(
            ITodoItemRepository repository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<TodoItemResponseDto>> GetAllAsync(TodoItemParameters parameters)
        {
            // 1. Gọi Repo: Lấy Tuple (danh sách gốc, tổng số lượng)
            // Cú pháp (var items, var totalCount) là cách "bóc tách" cái Tuple ra thành 2 biến riêng biệt.
            var (items, totalCount) = await _repository.GetAllAsync(parameters);

            // 2. Map sang DTO: Chuyển đổi dữ liệu thô sang dữ liệu đẹp để trả về
            var itemDtos = _mapper.Map<IEnumerable<TodoItemResponseDto>>(items);

            // 3. Đóng gói vào hộp PagedResult
            return new PagedResult<TodoItemResponseDto>
            {
                Items = itemDtos,           // Dữ liệu (Món hàng)
                TotalCount = totalCount,    // Tổng số lượng tìm thấy (Hóa đơn)
                PageNumber = parameters.PageNumber, // Trang hiện tại
                PageSize = parameters.PageSize      // Kích thước trang
            };
        }

        public async Task<TodoItemResponseDto> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return null;
            return _mapper.Map<TodoItemResponseDto>(item);
        }

        public async Task<TodoItemResponseDto> CreateAsync(CreateTodoItemRequestDto request)
        {
            // 1. Map DTO -> Entity
            var todoItem = _mapper.Map<TodoItem>(request);

            // 2. CHECK LOGIC: Nhờ ông hàng xóm kiểm tra xem CategoryId có thật không?
            var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId);
            if (!categoryExists)
            {
                // Nếu không tồn tại -> Ném lỗi ngay, không cho lưu
                throw new Exception($"Danh mục có ID = {request.CategoryId} không tồn tại!");
            }

            // 3. Lưu xuống DB
            await _repository.AddAsync(todoItem);

            // 4. MẸO QUAN TRỌNG:
            // Khi vừa Add xong, biến 'todoItem' chỉ có ID mới, nhưng 'Category' bên trong đang là null.
            // Nếu Map ngay bây giờ, 'CategoryName' sẽ bị rỗng.
            // -> Ta gọi lại hàm GetByIdAsync (đã có Include) để lấy dữ liệu đầy đủ nhất trả về cho khách.
            var freshItem = await _repository.GetByIdAsync(todoItem.Id);

            return _mapper.Map<TodoItemResponseDto>(freshItem);

        }

        public async Task<bool> UpdateAsync(int id, UpdateTodoItemRequestDto request)
        {
            var todoItem = await _repository.GetByIdAsync(id);
            if (todoItem == null) return false;

            // CHECK LOGIC: Nếu người dùng muốn đổi sang Category khác
            if (request.CategoryId != null)
            {
                var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId.Value);
                if (!categoryExists)
                {
                    throw new Exception($"Danh mục mới (ID = {request.CategoryId}) không tồn tại!");
                }
            }

            // Map đè dữ liệu mới (đã có cấu hình ignore null trong MappingProfile)
            _mapper.Map(request, todoItem);

            await _repository.UpdateAsync(todoItem);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todoItem = await _repository.GetByIdAsync(id);
            if (todoItem == null) return false;

            await _repository.DeleteAsync(todoItem);
            return true;
        }
    }
}


