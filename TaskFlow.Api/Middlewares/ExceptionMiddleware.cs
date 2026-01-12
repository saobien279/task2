using System.Net;
using System.Text.Json;

namespace TaskFlow.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Cho phép đi tiếp vào Controller
            }
            catch (Exception ex)
            {
                // Nếu Controller bị lỗi (ví dụ chia cho 0), nó sẽ văng xuống đây
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Lỗi 500


            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Hệ thống đang bận, vui lòng thử lại sau!",
                Detailed = exception.Message // Tạm thời hiện lỗi để mình debug
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
