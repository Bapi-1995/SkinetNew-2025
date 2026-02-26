import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { ShopComponent } from "./features/shop/shop.component";
import { ProductItemComponent } from './features/shop/product-item/product-item.component';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, ShopComponent,ProductItemComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class App{
 
  title = 'Skinet';

}
