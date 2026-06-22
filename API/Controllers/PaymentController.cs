
using Core.Entities;
using Core.Entities.OrderAggregates;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;
[Route("api/payments")]
public class PaymentsController(IPaymentServie paymentServie,IUnitOfWork unit,ILogger<PaymentsController> logger,
IConfiguration config,IHubContext<NotificationHub> hubContext) : BaseApiController
{

 private readonly string _whSecret=config["StripeSettings:WhSecret"]!;   
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        if (string.IsNullOrEmpty(cartId))
        {
            return BadRequest("Cart id is required");
        }

        try
        {
            var cart = await paymentServie.CreateOrUpdatePaymentIntent(cartId);
            if (cart == null) return BadRequest("Problem creating payment intent");
            return Ok(cart);
        }
        catch (StripeException ex)
        {
            return BadRequest($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Problem creating payment intent: {ex.Message}");
        }
    }
    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await unit.Repository<DeliveryMethod>().ListAllAsync());
    }
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();

        logger.LogInformation("Stripe webhook received. ContentLength={ContentLength}; ContentType={ContentType}; HasSignature={HasSignature}; WebhookSecretSet={HasSecret}",
            json?.Length ?? 0, Request.ContentType, !string.IsNullOrEmpty(signature), !string.IsNullOrEmpty(_whSecret));

        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(_whSecret))
        {
            logger.LogWarning("Stripe webhook request is missing signature or secret. Signature set: {HasSignature}, Secret set: {HasSecret}",
                !string.IsNullOrEmpty(signature), !string.IsNullOrEmpty(_whSecret));
            return BadRequest("Invalid webhook request");
        }

        try
        {
            var stripeEvent = ConstructStripeEvent(json, signature);
            logger.LogInformation("Received Stripe webhook event {EventType} for {EventId}", stripeEvent.Type, stripeEvent.Id);

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    logger.LogWarning("Stripe webhook event {EventType} had invalid data object of type {DataObjectType}", stripeEvent.Type,
                        stripeEvent.Data.Object?.GetType()?.Name ?? "null");
                    return BadRequest("Invalid event data");
                }

                await HandlePaymentIntentSucceeded(intent);
                return Ok();
            }

            logger.LogInformation("Ignoring Stripe webhook event type {EventType}", stripeEvent.Type);
            return Ok();
        }
        catch (StripeException ex) when (ex.Message.Contains("Invalid Signature", StringComparison.OrdinalIgnoreCase)
                                       || ex.Message.Contains("No signatures found", StringComparison.OrdinalIgnoreCase)
                                       || ex.Message.Contains("Unable to extract timestamp", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(ex, "Stripe webhook signature validation failed. SignatureHeader={Signature}", signature);
            return BadRequest("Invalid Stripe webhook signature");
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook processing failed");
            return StatusCode(StatusCodes.Status500InternalServerError, "Webhook processing failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status != "succeeded")
        {
            return;
        }

        var spec = new OrderSpecification(intent.Id, true);
        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
        if (order == null)
        {
            logger.LogWarning("Stripe webhook received for payment intent {PaymentIntentId}, but no order was found.", intent.Id);
            return;
        }

        if ((long)order.GetTotal() * 100 != intent.Amount)
        {
            order.Status = OrderStatus.PaymentMismatch;
        }
        else
        {
            order.Status = OrderStatus.PaymentReceived;
        }
        await unit.Complete();
        var connectionId=NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
        if (!string.IsNullOrEmpty(connectionId))
        {
            await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification",order.ToDto());
        }
    }

    private Event ConstructStripeEvent(string json, string signature)
    {
        try
        {
            return EventUtility.ConstructEvent(json, signature, _whSecret, throwOnApiVersionMismatch: false);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Failed to construct stripe event");
            throw;
        }
    }
}