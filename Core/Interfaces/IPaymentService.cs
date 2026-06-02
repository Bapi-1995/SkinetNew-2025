using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IPaymentServie
{
    Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string? cartId);
   

}
