using System.ComponentModel.DataAnnotations;


namespace TaskFlow.Api.DTOs
{
    public class TodoItemResponseDto
    {
            public int Id { get; set; }
            public string Title { get; set; }
            public bool IsCompleted { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } // Trả về tên danh mục cho tiện hiển thị
    }

    public class CreateTodoItemRequestDto
    {
        [Required(ErrorMessage = "Tiêu Đề Là Bắt Buộc")]
        [StringLength(100 , MinimumLength = 5 ,ErrorMessage = "Tiêu đề phải từ 5 đến 100 ký tự")]
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        [Range(1, int.MaxValue , ErrorMessage = "CategoryId Phải lớn hơn 0")]
        public int CategoryId { get; set; }
    }
}
