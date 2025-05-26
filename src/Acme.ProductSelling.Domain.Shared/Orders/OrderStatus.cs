namespace Acme.ProductSelling.Orders
{
    public enum OrderStatus
    {
        Placed,      // Đặt hàng thành công
        Pending,      // Mới đặt, chờ xử lý
        Confirmed,   // Đã xác nhận đơn hàng
        Processing,   // Đang xử lý (đóng gói,...)
        Shipped,      // Đã giao cho đơn vị vận chuyển
        Delivered,    // Đã giao thành công
        Cancelled
    }
}
