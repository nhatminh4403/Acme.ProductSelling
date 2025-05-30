namespace Acme.ProductSelling.Payments
{
    public enum PaymentStatus
    {
        Pending,      // Chờ thanh toán
        Completed,    // Đã thanh toán
        Failed,       // Thanh toán thất bại
        Refunded,     // Đã hoàn tiền
        Cancelled     // Đã hủy
    }
}
