import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { Product, product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class App implements OnInit{
  
  baseUrl='http://localhost:5029/api/';
  private http=inject(HttpClient);
  title = 'Skinet';
  products: Product[] = [];

  ngOnInit(): void {
    debugger;
     this.http.get<Pagination<Product>>(this.baseUrl+'product').subscribe({
      next: response=>this.products=response.data,
      error: error=>console.log(error),
      complete: ()=>console.log('Request completed')
      
      })
    
      
  }

}
