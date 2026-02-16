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
        [Required(ErrorMessage = "Tiêu đề công việc là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        [Range(1, int.MaxValue , ErrorMessage = "CategoryId Phải lớn hơn 0")]
        public int CategoryId { get; set; }
    }

    public class UpdateTodoItemRequestDto
    {

        [Required(ErrorMessage = "Tiêu đề công việc là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục không hợp lệ")]
        public int? CategoryId { get; set; }
    }
}
