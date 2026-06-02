import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, Form, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatError, MatInput, MatLabel } from '@angular/material/input';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule,MatFormFieldModule,MatInput,MatError,MatLabel],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.scss',
})
export class TextInputComponent implements ControlValueAccessor {
@Input() label='';
@Input() type='text';
constructor(@Self() public controlDir: NgControl) {
  this.controlDir.valueAccessor = this;
}


  writeValue(obj: any): void {
    
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }
  get control() {
    return this.controlDir.control as FormControl;
  }


}
