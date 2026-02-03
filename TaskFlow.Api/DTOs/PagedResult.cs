namespace TaskFlow.Api.DTOs
{
    // <T> là "ẩn số". Tí nữa mình truyền TodoItem vào thì T sẽ là TodoItem.
    public class PagedResult<T>
    {
        // 1. Dữ liệu chính (Món hàng)
        public IEnumerable<T> Items { get; set; }

        // 2. Thông tin phụ (Hóa đơn)
        public int TotalCount { get; set; }       // Tổng số bản ghi tìm thấy (Ví dụ: Tìm thấy 1000 kết quả)
        public int PageNumber { get; set; }       // Trang hiện tại (Ví dụ: Đang ở trang 1)
        public int PageSize { get; set; }         // Kích thước trang (Ví dụ: 10 dòng/trang)

        // 3. Tính toán tự động tổng số trang
        // Công thức: Tổng trang = Làm tròn lên (Tổng số lượng / Kích thước trang)
        // Ví dụ: Có 15 dòng, mỗi trang 10 dòng -> Cần 2 trang (1.5 làm tròn lên 2)
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}   