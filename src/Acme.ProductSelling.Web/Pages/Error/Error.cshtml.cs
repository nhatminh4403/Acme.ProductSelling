using Acme.ProductSelling.Web.Pages;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Views.Shared
{
    public class ErrorModel : ProductSellingPageModel
    {
        [BindProperty(SupportsGet = true)]
        public int StatusCode { get; set; }

        public string ErrorTitle { get; private set; }
        public string ErrorMessage { get; private set; }

        // The 'OnGet' method is executed when the page is requested
        public void OnGet()
        {
            switch (StatusCode)
            {
                case 401:
                    ErrorTitle = "Chưa xác thực";
                    ErrorMessage = "Bạn cần đăng nhập để xem trang này.";
                    break;
                case 403:
                    ErrorTitle = "Từ chối truy cập";
                    ErrorMessage = "Bạn không có quyền truy cập vào tài nguyên này.";
                    break;
                case 404:
                    ErrorTitle = "Không tìm thấy trang";
                    ErrorMessage = "Xin lỗi, trang bạn đang tìm kiếm không tồn tại.";
                    break;
                default:
                    // For status code 500 or any other unhandled errors
                    ErrorTitle = "Đã xảy ra lỗi!";
                    ErrorMessage = "Xin lỗi, đã có lỗi xảy ra trong quá trình xử lý yêu cầu của bạn.";
                    break;
            }
        }
    }
}
