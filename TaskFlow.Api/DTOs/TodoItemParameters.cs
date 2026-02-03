namespace TaskFlow.Api.DTOs
{
    public class TodoItemParameters
    {
        private const int MaxPageSize = 50; // Giới hạn tối đa để tránh user nhập 1000
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1; // Mặc định trang 1

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                // 'value' là con số mà người dùng đang cố gửi lên
                if (value > MaxPageSize)
                {
                    // Nếu đòi quá 50 -> Chỉ gán bằng 50 (Max)
                    _pageSize = MaxPageSize;
                }
                else
                {
                    // Nếu nhỏ hơn hoặc bằng 50 -> Cho phép gán đúng số họ đòi
                    _pageSize = value;
                }
            }
        }

        // 2. Tìm kiếm (theo tên công việc)
        public string SearchTerm { get; set; }

        // 3. Lọc (theo danh mục)
        public int? CategoryId { get; set; }

        // 6. Sắp xếp theo trường nào? (Mặc định là theo "id")
        public string SortBy { get; set; } = "id";

        // 7. Có sắp xếp giảm dần không? (Mặc định là False = Tăng dần)
        // Ví dụ: True = Z->A (hoặc Mới nhất -> Cũ nhất)
        public bool IsDescending { get; set; } = false;
    }
}
