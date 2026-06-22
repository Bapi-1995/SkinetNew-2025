import { inject, Injectable } from '@angular/core';
import { ConfirmationToken, loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElement, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { CartService } from './cart.service';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom, map } from 'rxjs';
import { Cart } from '../../shared/models/cart';
import { Address } from '../../shared/models/user';
import { setAlternateWeakRefImpl } from '@angular/core/primitives/signals';
import { AccountService } from './account.service';


@Injectable({
  providedIn: 'root',
})
export class StripeService {
  baseUrl=environment.apiUrl;
  private cartService=inject(CartService);
  private accountService=inject(AccountService);
  private http=inject(HttpClient);
  private stripePromise?: Promise<Stripe | null>;
  private elements?:StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElemnet?:StripePaymentElement;

  constructor(){
    this.stripePromise=loadStripe(environment.stripePublicKey);
  }
  getStripeInstance(){
    return this.stripePromise;
  }
  async initilizeElements(){
   if(!this.elements){
    const stripe=await this.getStripeInstance();
    if(stripe){
      const cart=await firstValueFrom(this.CreateOrderPaymentIntent());
      this.elements=stripe.elements(
        {clientSecret:cart.clientSecret,appearance:{labels:'floating'}})
      }
        else{
          throw new Error('Stripe failed to initialize');
        }
  
  }
  return this.elements;
}
async createAddressElement(){
  if(!this.addressElement){
    const elements=await this.initilizeElements();
    if(elements){
      const user=this.accountService.currentUser();
      let deafultValues:StripeAddressElementOptions['defaultValues']={};
      if(user){
        deafultValues.name=user.firstName + ' ' + user.lastName;
      }
      if(user?.address){
        deafultValues.address={
          line1:user.address.line1,
          line2:user.address.line2,
          city:user.address.city,
          state:user.address.state,
          postal_code:user.address.postalCode,
          country:user.address.country
        }

      }
      const options:StripeAddressElementOptions={
        mode:'shipping',
        defaultValues:deafultValues
    };
    this.addressElement=elements.create('address',options);
  }else{
    throw new Error('Elemets instance has not been loaded');
  }
}
return this.addressElement;
}
async createPaymentElement(){
  if(!this.paymentElemnet){
    const elements=await this.initilizeElements(); 
   if(elements){
    this.paymentElemnet=elements.create('payment');
   }
   else{
    throw new  Error('Elemets instance has not been loaded');
   }
  }
  return this.paymentElemnet;
}
async  createConfirmationToken(){
  const stripe=await this.getStripeInstance();
  const elements=await this.initilizeElements();
  const result=await elements.submit();
  if(result.error) throw new Error(result.error.message);
  if(stripe){
    return await stripe.createConfirmationToken({elements});
  
  }
  else{
    throw new Error('Stripe not avaliable');
  }

}
async confirmPayment(confirmationToken:ConfirmationToken){
  const stripe=await this.getStripeInstance();
  const clientSecret=this.cartService.cart()?.clientSecret;
  if(stripe && clientSecret){
    return await stripe.confirmPayment({
      clientSecret: clientSecret,
      confirmParams: {
        confirmation_token: confirmationToken.id,
        return_url: window.location.href,
      },
      redirect: 'if_required',
    });
  }
  else{
    throw new Error('Unable to load stripe or client secret');
  }
}
  CreateOrderPaymentIntent(){
    const cart=this.cartService.cart();
    if(!cart) throw new Error('Problem with the cart');
    return this.http.post<Cart>(this.baseUrl+'payments/'+ cart.id,{}).pipe(
      map(cart =>{
        this.cartService.setCart(cart);
        return cart;
      })
    )
  }
  disposeElements(){
    this.addressElement=undefined;
    this.elements=undefined;
    this.paymentElemnet=undefined;
  }

  
}
