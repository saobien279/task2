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
        private readonly ICategoryRepository _categoryRepository;
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

        public async Task<PagedResult<TodoItemResponseDto>> GetAllAsync(TodoItemParameters parameters, int userId)
        {
            // Truyền userId xuống Repo
            var (items, totalCount) = await _repository.GetAllAsync(parameters, userId);

            var itemDtos = _mapper.Map<IEnumerable<TodoItemResponseDto>>(items);

            return new PagedResult<TodoItemResponseDto>
            {
                Items = itemDtos,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

        public async Task<TodoItemResponseDto> GetByIdAsync(int id, int userId)
        {
            // Tìm item của đúng user đó
            var item = await _repository.GetByIdAsync(id, userId);
            if (item == null) return null;

            return _mapper.Map<TodoItemResponseDto>(item);
        }

        public async Task<TodoItemResponseDto> CreateAsync(CreateTodoItemRequestDto request, int userId)
        {
            var todoItem = _mapper.Map<TodoItem>(request);

            // 1. CHECK LOGIC: Category có tồn tại không?
            var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId);
            if (!categoryExists)
            {
                throw new Exception($"Danh mục có ID = {request.CategoryId} không tồn tại!");
            }

            // 2. QUAN TRỌNG: Gán chủ sở hữu cho công việc mới
            todoItem.UserId = userId;

            // 3. Lưu
            await _repository.AddAsync(todoItem);

            // 4. Lấy lại dữ liệu đầy đủ để trả về (kèm tên Category)
            var freshItem = await _repository.GetByIdAsync(todoItem.Id, userId);
            return _mapper.Map<TodoItemResponseDto>(freshItem);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTodoItemRequestDto request, int userId)
        {
            // 1. Tìm công việc (phải đúng của userId này mới tìm thấy)
            var todoItem = await _repository.GetByIdAsync(id, userId);
            if (todoItem == null) return false;

            // 2. Nếu đổi Category, check tồn tại
            if (request.CategoryId != null)
            {
                var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId.Value);
                if (!categoryExists)
                {
                    throw new Exception($"Danh mục mới (ID = {request.CategoryId}) không tồn tại!");
                }
            }

            // 3. Map đè dữ liệu mới
            _mapper.Map(request, todoItem);

            await _repository.UpdateAsync(todoItem);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            // 1. Tìm công việc (đúng chủ mới xóa được)
            var todoItem = await _repository.GetByIdAsync(id, userId);
            if (todoItem == null) return false;

            await _repository.DeleteAsync(todoItem);
            return true;
        }
    }
}