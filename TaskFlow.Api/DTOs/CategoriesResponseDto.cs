using System.ComponentModel.DataAnnotations;
using TaskFlow.Api.Validations;

namespace TaskFlow.Api.DTOs
{
    public class CategoriesResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TotalItem { get; set; }
    }

    public class CreateCategoryRequestDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(100, MinimumLength = 5 , ErrorMessage = "Tiêu đề phải từ 5 đến 100 ký tự")]
        [NoAdmin]
        public string Name { get; set; }
    }
}
