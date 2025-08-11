namespace Acme.ProductSelling.Payments
{
    public enum PaymentStatus
    {
        Unpaid,       // Chưa thanh toán
        PendingOnDelivery, // Chờ thanh toán khi giao hàng
        Pending,      // Chờ thanh toán
        Paid,    // Đã thanh toán
        Failed,       // Thanh toán thất bại
        Refunded,     // Đã hoàn tiền
        Cancelled     // Đã hủy
    }
}
