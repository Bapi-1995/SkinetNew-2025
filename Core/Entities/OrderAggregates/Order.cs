using System;
using Core.Entities;
using Core.Entities.OrderAggregates;

public class Order:BaseEntity
{
    public ShippingAddress ShippingAddress { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public PaymentSummery PaymentSummery { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public decimal Subtotal { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public required string PaymentIntentId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string BuyerEmail { get; set; } = string.Empty;

    public decimal GetTotal()
    {
        return Subtotal + DeliveryMethod.Price;
    }

}