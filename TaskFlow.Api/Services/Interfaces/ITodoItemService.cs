using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services.Interfaces
{
    public interface ITodoItemService
    {
        Task<PagedResult<TodoItemResponseDto>> GetAllAsync(TodoItemParameters parameters);
        Task<TodoItemResponseDto> GetByIdAsync(int id);

        Task<TodoItemResponseDto> CreateAsync(CreateTodoItemRequestDto request);
        Task<bool> UpdateAsync(int id, UpdateTodoItemRequestDto request);
        Task<bool> DeleteAsync(int id);
    }
}
