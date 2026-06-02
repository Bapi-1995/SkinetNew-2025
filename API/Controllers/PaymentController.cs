
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers;
[Route("api/payments")]
public class PaymentsController(IPaymentServie paymentServie,IGenericRepository<DeliveryMethod> dmRepo) : BaseApiController
{
    
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
        return Ok(await dmRepo.ListAllAsync());
    }

    
}