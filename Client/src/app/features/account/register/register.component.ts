import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatInput } from '@angular/material/input';
import { MatError, MatFormField, MatLabel } from '@angular/material/select';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { SnacbarService } from '../../../core/services/snacbar.service';
import { JsonPipe } from '@angular/common';
import { TextInputComponent } from "../../../shared/components/text-input/text-input.component";

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatLabel,
    MatInput,
    MatButton,
    JsonPipe,
    MatError,
    TextInputComponent
],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
private fb=inject(FormBuilder);
private accountService=inject(AccountService);
private router=inject(Router);
private snack=inject(SnacbarService);
validationErrors?:string[]
registerForm=this.fb.group({
  firstName:['',Validators.required],
  lastName:['',Validators.required],
  email:['',[Validators.required,Validators.email]],
  password:['',Validators.required]
});
onSubmit(){
  this.accountService.register(this.registerForm.value).subscribe({
    next:()=>{
      this.snack.success('Registration successfull - you can now login');
      this.router.navigateByUrl('/login');
    },
    error:errors=>this.validationErrors=errors
  })
}

}
