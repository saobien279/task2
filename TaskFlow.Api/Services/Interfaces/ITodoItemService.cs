using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services.Interfaces
{
    public interface ITodoItemService
    {
        Task<PagedResult<TodoItemResponseDto>> GetAllAsync(TodoItemParameters parameters, int userId);
        Task<TodoItemResponseDto> GetByIdAsync(int id, int userId);

        Task<TodoItemResponseDto> CreateAsync(CreateTodoItemRequestDto request, int userId);
        Task<bool> UpdateAsync(int id, UpdateTodoItemRequestDto request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
