namespace Core.Entities.OrderAggregates;
public enum OrderStatus
{
    Pending,
    PaymentReceived,
    PaymentFailed,
    PaymentMismatch
}