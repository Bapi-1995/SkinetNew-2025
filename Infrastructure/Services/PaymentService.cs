
using System.Security.Cryptography.X509Certificates;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services;

public class PaymentService : IPaymentServie
{
    private readonly IConfiguration _config;
    private readonly ICartService _cartService;
    private readonly IGenericRepository<Core.Entities.Product> _productRepo;
    private readonly IGenericRepository<DeliveryMethod> _dmRepo;

    public PaymentService(IConfiguration config, ICartService cartService,
        IGenericRepository<Core.Entities.Product> productRepo,
        IGenericRepository<DeliveryMethod> dmRepo)
    {
        _config = config;
        _cartService = cartService;
        _productRepo = productRepo;
        _dmRepo = dmRepo;
    }

    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string? cartId)
    {
        if (string.IsNullOrEmpty(cartId)) return null;

        StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null) return null;

        var shippingPrice = 0m;
        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await _dmRepo.GetByIdAsync((int)cart.DeliveryMethodId);
            if (deliveryMethod == null) return null;
            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in cart.Items)
        {
            var productItem = await _productRepo.GetByIdAsync(item.ProductId);
            if (productItem == null) return null;
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }

        var total = cart.Items.Sum(x => x.Quantity * (x.Price * 100m)) + shippingPrice * 100m;
        if (total <= 0) return null; // Invalid amount

        var service = new PaymentIntentService();
        PaymentIntent intent;
        var amount = Convert.ToInt64(Math.Round(total, 0));

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };
            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            PaymentIntent? existingIntent = null;
            try
            {
                existingIntent = await service.GetAsync(cart.PaymentIntentId);
            }
            catch (StripeException)
            {
                existingIntent = null;
            }

            if (existingIntent == null || existingIntent.Status == "succeeded" || existingIntent.Status == "canceled")
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = amount
                };
                intent = await service.UpdateAsync(cart.PaymentIntentId, options);
            }
        }

        await _cartService.SetCartAsync(cart);
        return cart;
    }
}