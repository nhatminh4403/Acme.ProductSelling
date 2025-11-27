using System;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Payments;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Orders;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class OrderMapper : MapperBase<Order, OrderDto>
{
    [MapProperty(nameof(Order.OrderStatus), nameof(OrderDto.OrderStatus))]
    [MapProperty(nameof(Order.PaymentStatus), nameof(OrderDto.PaymentStatus))]
    [MapProperty(nameof(Order.OrderStatus), nameof(OrderDto.OrderStatusText))]
    [MapProperty(nameof(Order.PaymentStatus), nameof(OrderDto.PaymentStatusText))]
    [MapperIgnoreTarget(nameof(OrderDto.StoreName))]
    public override partial OrderDto Map(Order source);

    public override void Map(Order source, OrderDto destination)
    {
        throw new NotImplementedException();
    }

    // Helpers for Enum text mapping
    private string MapEnumToString(OrderStatus status) => status.ToString();
    private string MapEnumToString(PaymentStatus status) => status.ToString();
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateOrderMapper : MapperBase<CreateOrderDto, Order>
{
    [ObjectFactory]
    Order Create(CreateOrderDto dto)
    {
        // Placeholder values for OrderNumber/CustomerId are used.
        // Your Domain Service MUST override these with generated/current user logic after mapping.
        var type = dto.OrderType ?? OrderType.Online;

        return new Order(
            Guid.NewGuid(),
            "PENDING_GEN",
            DateTime.Now,
            null, // CustomerId
            dto.CustomerName,
            dto.CustomerPhone,
            dto.ShippingAddress,
            dto.PaymentMethod
        );
    }

    [MapperIgnoreTarget(nameof(Order.Id))]
    [MapperIgnoreTarget(nameof(Order.OrderNumber))]
    [MapperIgnoreTarget(nameof(Order.OrderDate))]
    [MapperIgnoreTarget(nameof(Order.CustomerId))]
    [MapperIgnoreTarget(nameof(Order.TotalAmount))]
    [MapperIgnoreTarget(nameof(Order.OrderStatus))]
    [MapperIgnoreTarget(nameof(Order.PaymentStatus))]
    [MapperIgnoreTarget(nameof(Order.StoreId))]
    [MapperIgnoreTarget(nameof(Order.SellerId))]
    [MapperIgnoreTarget(nameof(Order.SellerName))]
    [MapperIgnoreTarget(nameof(Order.CashierId))]
    [MapperIgnoreTarget(nameof(Order.CashierName))]
    [MapperIgnoreTarget(nameof(Order.FulfillerId))]
    [MapperIgnoreTarget(nameof(Order.FulfillerName))]
    [MapperIgnoreTarget(nameof(Order.CompletedAt))]
    [MapperIgnoreTarget(nameof(Order.FulfilledAt))]
    [MapperIgnoreTarget(nameof(Order.OrderType))] // Handled in Ctor or skipped

    [MapperIgnoreTarget(nameof(Order.OrderItems))]
    [MapperIgnoreTarget(nameof(Order.OrderHistories))]

    [MapperIgnoreTarget(nameof(Order.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Order.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Order.CreationTime))]
    [MapperIgnoreTarget(nameof(Order.CreatorId))]
    [MapperIgnoreTarget(nameof(Order.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Order.LastModifierId))]
    [MapperIgnoreTarget(nameof(Order.IsDeleted))]
    [MapperIgnoreTarget(nameof(Order.DeleterId))]
    [MapperIgnoreTarget(nameof(Order.DeletionTime))]
    public override partial Order Map(CreateOrderDto source);

    public override void Map(CreateOrderDto source, Order destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderItemMapper
{
    [ObjectFactory]
    OrderItem Create(CreateOrderItemDto dto)
    {
        // IMPORTANT: ProductName and Price are missing in DTO. 
        // Logic requires passing these. Domain Service must re-set or inject Product logic.
        return new OrderItem(
            Guid.Empty,
            dto.ProductId,
            "TEMP_NAME",
            0,
            dto.Quantity
        );
    }

    [MapperIgnoreTarget(nameof(OrderItem.Id))]
    [MapperIgnoreTarget(nameof(OrderItem.OrderId))]
    [MapperIgnoreTarget(nameof(OrderItem.LineTotalAmount))]
    [MapperIgnoreTarget(nameof(OrderItem.ProductName))]
    [MapperIgnoreTarget(nameof(OrderItem.Price))]
    public partial OrderItem Map(CreateOrderItemDto source);

    public partial OrderItemDto Map(OrderItem source);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderHistoryMapper : MapperBase<OrderHistory, OrderHistoryDto>
{
    public override partial OrderHistoryDto Map(OrderHistory source);

    public override void Map(OrderHistory source, OrderHistoryDto destination)
    {
        throw new NotImplementedException();
    }
}