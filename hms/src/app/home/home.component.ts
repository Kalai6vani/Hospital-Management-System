import { Component } from '@angular/core';

//add imports
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  isLoggedIn=false;
  userRole: string | null=null;
  constructor(private router: Router, private authService: AuthService){}

  navigateToLogin(){
    this.router.navigate(['/login']);
  }

  navigateToRegister(){
    this.router.navigate(['/register']);
  }

  logout():void{
this.authService.logout();
this.updateLoginStatus();
this.router.navigate(['/']);
  }

  updateLoginStatus():void{
    this.isLoggedIn=this.authService.isLoggedIn();
    this.userRole=this.authService.getUserRole();
  }
}
