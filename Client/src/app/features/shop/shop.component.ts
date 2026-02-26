import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { MatCard } from '@angular/material/card';
import { ProductItemComponent } from "./product-item/product-item.component";
import { NgForOf } from "../../../../node_modules/@angular/common/types/_common_module-chunk";
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters/filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { ShopParams } from '../../shared/models/shopParams';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-shop',
  imports: [MatCard, 
            ProductItemComponent,
            MatButton,
            MatIcon,
            MatMenu,
            MatSelectionList,
            MatListOption,
            MatMenuTrigger,
            MatPaginator,FormsModule],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})

export class ShopComponent implements OnInit {
 private shopService=inject(ShopService);
 private dialogService=inject(MatDialog);
  
 sortOptions=[
  {name:'Alphabetical',value:'name'},
  {name:'Price: Low to High',value:'priceAsc'},
  {name:'Price: High to Low',value:'priceDesc'},
 ]
 shopParams=new ShopParams();
 pageSizeOptions=[5,10,20,50];

 products?: Pagination<Product>;
  ngOnInit(): void {
      this.intializeShop();
    
    }
    intializeShop(){
      this.shopService.getBrands();
      this.shopService.getTypes();
      this.getProducts();
    }
    getProducts(){
       this.shopService.getProducts(this.shopParams).subscribe({
      next: response=>this.products=response,
      error: error=>console.log(error),
      
      })
    }
    handlePageEvent(event:PageEvent){
      this.shopParams.pageNumber=event.pageIndex+1;
      this.shopParams.pageSize=event.pageSize;
      this.getProducts();
    }
    onSortChange(event:any){
      debugger;
      const selectedOption=event.options[0].value;
      if(selectedOption)
      this.shopParams.sort=selectedOption;
      this.shopParams.pageNumber=1;
      this.getProducts();
    }
    onSearchChange(){
      this.shopParams.pageNumber=1;
      this.getProducts();
      }
    openFilterDialog(){
      const dialogRef=this.dialogService.open(FiltersDialogComponent,{
        minWidth:'500px',
        maxHeight: '80vh',
      data:{
        selectedBrands:this.shopParams.brands,
        selectedTypes:this.shopParams.types,
      }
      
      
      });
      
      dialogRef.afterClosed().subscribe({
        next:result=>{
          if(result){
            console.log(result);
            this.shopParams.brands=result.selectedBrands;
            this.shopParams.types=result.selectedTypes;
            this.shopParams.pageNumber=1;
            //apply filters
            this.getProducts();
          }
        }
      })

    }
    
}
