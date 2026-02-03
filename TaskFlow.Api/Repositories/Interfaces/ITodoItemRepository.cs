using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Repositories.Interfaces
{
    // LƯU Ý: Phải là interface, không phải class
    public interface ITodoItemRepository
    {
        Task<(IEnumerable<TodoItem>, int)> GetAllAsync(TodoItemParameters parameters);
        Task<TodoItem> GetByIdAsync(int id);
        Task AddAsync(TodoItem todoItem);
        Task UpdateAsync(TodoItem todoItem);
        Task DeleteAsync(TodoItem todoItem);
    }
}