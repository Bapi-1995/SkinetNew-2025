import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Order, OrderToCreate } from '../../shared/models/Order';

@Injectable({
  providedIn: 'root',
})
export class OrderService {

  baseUrl = environment.apiUrl;
  private http=inject(HttpClient);
  orderComplete=false;

  createOrder(orderToCreate: OrderToCreate) {
    debugger;
    return this.http.post(this.baseUrl+'Orders', orderToCreate, { withCredentials: true });
}
getOrdersForUser() {
 return this.http.get<Order[]>(this.baseUrl+'Orders', { withCredentials: true });
}

getOrderDetails(id: number) {
  return this.http.get<Order>(this.baseUrl + 'orders/' + id);
}
  
}
