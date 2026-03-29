using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Payments;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Orders;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class OrderToOrderDtoMapper : MapperBase<Order, OrderDto>
{
    [MapperIgnoreTarget(nameof(OrderDto.OrderStatusText))]
    [MapperIgnoreTarget(nameof(OrderDto.PaymentStatusText))]
    [MapperIgnoreTarget(nameof(OrderDto.StoreName))]
    // OnlineOrder-only
    [MapperIgnoreTarget(nameof(OrderDto.ShippingAddress))]
    // InStoreOrder-only
    [MapperIgnoreTarget(nameof(OrderDto.StoreId))]
    [MapperIgnoreTarget(nameof(OrderDto.SellerId))]
    [MapperIgnoreTarget(nameof(OrderDto.SellerName))]
    [MapperIgnoreTarget(nameof(OrderDto.CashierId))]
    [MapperIgnoreTarget(nameof(OrderDto.CashierName))]
    [MapperIgnoreTarget(nameof(OrderDto.FulfillerId))]
    [MapperIgnoreTarget(nameof(OrderDto.FulfillerName))]
    [MapperIgnoreTarget(nameof(OrderDto.CompletedAt))]
    [MapperIgnoreTarget(nameof(OrderDto.FulfilledAt))]
    public override partial OrderDto Map(Order source);

    [MapperIgnoreTarget(nameof(OrderDto.OrderStatusText))]
    [MapperIgnoreTarget(nameof(OrderDto.PaymentStatusText))]
    [MapperIgnoreTarget(nameof(OrderDto.StoreName))]
    [MapperIgnoreTarget(nameof(OrderDto.ShippingAddress))]
    [MapperIgnoreTarget(nameof(OrderDto.StoreId))]
    [MapperIgnoreTarget(nameof(OrderDto.SellerId))]
    [MapperIgnoreTarget(nameof(OrderDto.SellerName))]
    [MapperIgnoreTarget(nameof(OrderDto.CashierId))]
    [MapperIgnoreTarget(nameof(OrderDto.CashierName))]
    [MapperIgnoreTarget(nameof(OrderDto.FulfillerId))]
    [MapperIgnoreTarget(nameof(OrderDto.FulfillerName))]
    [MapperIgnoreTarget(nameof(OrderDto.CompletedAt))]
    [MapperIgnoreTarget(nameof(OrderDto.FulfilledAt))]
    public override partial void Map(Order source, OrderDto destination);

    public override void AfterMap(Order source, OrderDto destination)
    {
        // Shared computed fields
        destination.OrderStatusText = source.OrderStatus.ToString();
        destination.PaymentStatusText = source.PaymentStatus.ToString();

        // Subtype-specific fields
        switch (source)
        {
            case OnlineOrder online:
                destination.ShippingAddress = online.ShippingAddress;
                break;

            case InStoreOrder inStore:
                destination.StoreId = inStore.StoreId;
                destination.SellerId = inStore.SellerId;
                destination.SellerName = inStore.SellerName;
                destination.CashierId = inStore.CashierId;
                destination.CashierName = inStore.CashierName;
                destination.FulfillerId = inStore.FulfillerId;
                destination.FulfillerName = inStore.FulfillerName;
                destination.CompletedAt = inStore.CompletedAt;
                destination.FulfilledAt = inStore.FulfilledAt;
                break;
        }
    }
    public partial List<OrderDto> MapList(List<Order> orders);

}
[Mapper]
public partial class OrderListMapper
{
    private readonly OrderToOrderDtoMapper _orderMapper;

    public OrderListMapper(OrderToOrderDtoMapper orderMapper)
    {
        _orderMapper = orderMapper;
    }

    public List<OrderDto> MapList(List<Order> orders)
    {
        var result = new List<OrderDto>();
        foreach (var order in orders)
        {
            result.Add(_orderMapper.Map(order));
        }
        return result;
    }

    // Alternative if you want partial method
    //public partial List<OrderDto> MapList(List<Order> orders);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderItemToOrderItemDtoMapper : MapperBase<OrderItem, OrderItemDto>
{
    public override partial OrderItemDto Map(OrderItem source);
    public override partial void Map(OrderItem source, OrderItemDto destination);
}



[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateOrderDtoToOnlineOrderMapper : MapperBase<CreateOrderDto, OnlineOrder>
{
    public override OnlineOrder Map(CreateOrderDto source)
    {

        var entity = new OnlineOrder(
            Guid.Empty,       // replaced by GuidGenerator.Create() in app service
            string.Empty,     // replaced by order-number generation in app service
            DateTime.UtcNow,  // replaced by Clock.Now in app service
            null,             // CustomerId — set from CurrentUser in app service
            source.CustomerName,
            source.CustomerPhone,
            source.ShippingAddress,
            source.PaymentMethod
        );

        Map(source, entity);
        return entity;
    }
    [MapperIgnoreTarget(nameof(OnlineOrder.Id))]
    [MapperIgnoreTarget(nameof(OnlineOrder.OrderNumber))]
    [MapperIgnoreTarget(nameof(OnlineOrder.OrderDate))]
    [MapperIgnoreTarget(nameof(OnlineOrder.CustomerId))]
    [MapperIgnoreTarget(nameof(OnlineOrder.CustomerName))]
    [MapperIgnoreTarget(nameof(OnlineOrder.CustomerPhone))]
    [MapperIgnoreTarget(nameof(OnlineOrder.ShippingAddress))]
    [MapperIgnoreTarget(nameof(OnlineOrder.PaymentMethod))]
    [MapperIgnoreTarget(nameof(OnlineOrder.OrderStatus))]
    [MapperIgnoreTarget(nameof(OnlineOrder.PaymentStatus))]
    [MapperIgnoreTarget(nameof(OnlineOrder.OrderType))]
    [MapperIgnoreTarget(nameof(OnlineOrder.TotalAmount))]
    [MapperIgnoreTarget(nameof(OnlineOrder.OrderItems))]
    [MapperIgnoreTarget(nameof(OnlineOrder.OrderHistories))]
    public override partial void Map(CreateOrderDto source, OnlineOrder destination);
}


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderHistoryToOrderHistoryDtoMapper : MapperBase<OrderHistory, OrderHistoryDto>
{
    public override partial OrderHistoryDto Map(OrderHistory source);

    public override partial void Map(OrderHistory source, OrderHistoryDto destination);

    private string MapStatus(OrderStatus status) => status.ToString();
    private string MapPayStatus(PaymentStatus status) => status.ToString();
}
[Mapper]
public partial class OrderHistoryListMapper
{
    private readonly OrderHistoryToOrderHistoryDtoMapper _historyMapper;

    public OrderHistoryListMapper(OrderHistoryToOrderHistoryDtoMapper historyMapper)
    {
        _historyMapper = historyMapper;
    }

    public List<OrderHistoryDto> MapList(List<OrderHistory> histories)
    {
        var result = new List<OrderHistoryDto>();
        foreach (var history in histories)
        {
            result.Add(_historyMapper.Map(history));
        }
        return result;
    }
}