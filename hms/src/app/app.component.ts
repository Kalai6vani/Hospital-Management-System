import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

//add imports
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'hms';

  isLoggedIn=false;
  role='';

  constructor(private router: Router){}

  ngOnInit(){
    this.checkLoginStatus();
  }

  checkLoginStatus(){
    const storedUser=localStorage.getItem('user');
    if(storedUser){
      const user=JSON.parse(storedUser);
      this.isLoggedIn=true;
      this.role=user.role;
    }
  }

  logout(){
    localStorage.removeItem('user');
    this.isLoggedIn=false;
    this.role='';
    this.router.navigate(['/login']);
  }
}
