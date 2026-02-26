import { Component, inject } from '@angular/core';
import { ShopService } from '../../../../core/services/shop.service';
import { MatDivider } from '@angular/material/divider';
import { MatSelectionList, MatListOption } from '@angular/material/list';
import { MatOption } from '@angular/material/autocomplete';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';

@Component({
  selector: 'app-filters-dialog',
  imports: [MatDivider, MatSelectionList, MatListOption,MatButton,FormsModule],
  templateUrl: './filters-dialog.component.html',
  styleUrl: './filters-dialog.component.scss',
})
export class FiltersDialogComponent {
 shopService=inject(ShopService);
 private dialogref=inject(MatDialogRef<FiltersDialogComponent>);
 data=inject(MAT_DIALOG_DATA);
  selectedBrands:string[]=this.data.selectedBrands;
  selectedTypes:string[]=this.data.selectedTypes;
  applyFilters(){
    this.dialogref.close({
      selectedBrands:this.selectedBrands,
      selectedTypes:this.selectedTypes,
    })
  }

}
