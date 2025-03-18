import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
 selector: 'app-login',
 standalone: true,
 imports: [CommonModule, FormsModule],
 templateUrl: './login.component.html',
 styleUrls: ['./login.component.css']
})

export class LoginComponent {
 loginData = {
   email: '',
   password: ''
 };
 errorMessage: string = '';

 constructor(private router: Router, private authService: AuthService) {} // Inject AuthService
 
 onLogin(): void {
   this.authService.login(this.loginData).subscribe({
     next: (response) => 
       console.log('Login successful:', response),
       error: (err) => this.errorMessage='Invalid credentials'
   });
  }
  
  navigateToRegister(){
    this.router.navigate(['/register']);
  }
}
 