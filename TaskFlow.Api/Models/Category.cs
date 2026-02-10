using System.Collections.Generic;

namespace TaskFlow.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        public int UserId { get; set; }
    }
}
