using System.ComponentModel.DataAnnotations;
using Core.Entities.OrderAggregates;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class CreateOrderDto
{
    public string CartId{get;set;}=string.Empty;
    [Required]
    public int DeliveryMethodId { get; set; }
    [Required]
    public ShippingAddress ShippingAddress { get; set; }=null!;
    [Required]
    public PaymentSummery PaymentSummary { get; set; }=null!;

}