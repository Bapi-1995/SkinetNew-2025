using Core.Entities.OrderAggregates;

namespace API.DTOs
{
    public class OrderDto
    {
                public int Id{get;set;}
                public DateTime OrderDate { get; set; }
                 public string BuyerEmail { get; set; } = string.Empty;
                public required ShippingAddress ShippingAddress { get; set; }
                public required string DeliveryMethod { get; set; }
                public decimal ShippingPrice { get; set; }
                public required PaymentSummery PaymentSummery { get; set; }
                public List<OrderItemDto> OrderItems { get; set; }
                public decimal Subtotal { get; set; }
                public required string Status { get; set; }
                public decimal Total { get; set; }
                public required string PaymentIntentId { get; set; }
               

    }
}