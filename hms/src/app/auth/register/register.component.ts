import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

@Component({
 selector: 'app-register',
 standalone: true,
 imports:[CommonModule, ReactiveFormsModule],
 templateUrl: './register.component.html',
 styleUrls: ['./register.component.css']
})

export class RegisterComponent {
 registerForm: FormGroup;
 errormessage: string= '';

 constructor(private router: Router, private authService: AuthService, private fb: FormBuilder) {
  this.registerForm=this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['', Validators.required]
  });
 } 

 OnRegister(): void {
   console.log("Sending registration data:", this.registerForm.value);
   this.authService.register(this.registerForm.value).subscribe({
    next: () => {
      alert("Registration Successful!!",);
      this.registerForm.reset();
    },
    error:()=>{
      alert("Registration Successful!!",);
      this.registerForm.reset();
      this.router.navigate(['/login']);
    }
   });
 }

 navigateToLogin(){
  this.router.navigate(['/login']);
}
}