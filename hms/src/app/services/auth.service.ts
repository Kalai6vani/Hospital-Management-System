import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root' // Provided globally
})
export class AuthService {
  private baseUrl = 'https://localhost:7003/api/Auth'; // Your backend API

  constructor(private http: HttpClient, private router: Router) {}

  // User Login with Role-Based Navigation
  login(loginData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, loginData).pipe(
      tap((response: any) => {
        if (response.token && response.role) {
          localStorage.setItem('token', response.token);
          localStorage.setItem('role', response.role);
          localStorage.setItem('userId', response.userId);
          console.log("Role Stored:", response.role);

          
          this.redirectUser(response.role);
        }
      })
    );
  }

  private redirectUser(role: string): void {
    // Redirect to the respective dashboard based on role
    switch (role.toLowerCase()) {
      case 'admin':
        this.router.navigate(['/admin-dashboard']);
        break;
      case 'doctor':
        this.router.navigate(['/doctor-dashboard']);
        break;
      case 'patient':
        this.router.navigate(['/patient-dashboard']);
        break;
      default:
        this.router.navigate(['/']);
    }
  }

  // User Registration
  register(registerData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, registerData, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      tap(() => {
        this.router.navigate(['/login']);
      })
    );
  }

  // Check if user is logged in
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  // Get user role
  getUserRole(): string | null {
    return localStorage.getItem('role');
  }

  // Logout
  logout(): void {
    localStorage.clear();
    this.router.navigate(['/']);
  }

  // Check token and handle redirection
  checkToken(): void {
    const token = localStorage.getItem('token');
    if (!token) {
      this.router.navigate(['/login']);
    }
  }

  // Handle HTTP errors
  handleError(error: HttpErrorResponse): Observable<never> {
    if (error.status === 401) {
      // Token is invalid or expired, redirect to login
      localStorage.removeItem('token');
      this.router.navigate(['/login']);
    }
    return throwError(error);
  }
}