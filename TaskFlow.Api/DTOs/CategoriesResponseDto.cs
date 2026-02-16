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
        [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự")]
        //[NoAdmin]
        public string Name { get; set; }
    }

    public class UpdateCategoryRequestDto
    {
        [Required(ErrorMessage = "Bắt buộc phải có ID")]
        public int ? id  { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự")]
        public string Name { get; set; }

    }
}
