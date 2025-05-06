namespace Acme.ProductSelling.Orders
{
    public enum OrderStatus
    {
        Pending,      // Mới đặt, chờ xử lý
        Processing,   // Đang xử lý (đóng gói,...)
        Shipped,      // Đã giao cho đơn vị vận chuyển
        Delivered,    // Đã giao thành công
        Cancelled
    }
}
