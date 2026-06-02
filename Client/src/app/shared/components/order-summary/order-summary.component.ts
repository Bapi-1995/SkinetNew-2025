import { Component, inject, Inject } from '@angular/core';
import { RouterLink } from "@angular/router";
import { MatAnchor } from "@angular/material/button";
import { MatFormField, MatLabel } from "@angular/material/select";
import { MatInput } from "@angular/material/input";
import { CartService } from '../../../core/services/cart.service';
import { CurrencyPipe, Location } from '@angular/common';

@Component({
  selector: 'app-order-summary',
  imports: [RouterLink, MatAnchor, MatFormField, MatLabel, MatInput,CurrencyPipe],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss',
})
export class OrderSummaryComponent {

    cartService=inject(CartService);
    location=inject(Location);
}
