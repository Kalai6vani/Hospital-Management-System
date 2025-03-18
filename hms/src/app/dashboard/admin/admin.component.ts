import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgIf, NgFor, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin',
  imports: [NgIf, NgFor, NgClass, FormsModule],
  standalone: true,
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  hospitals: any[] = [];
  assets: any[] = [];
  medicines: any[] = [];
  apiBaseUrl = 'https://localhost:7003/api'; // Replace with your API URL

  // Modal State
  showModal: boolean = false;
  selectedItem: any = {};
  entityType: string = '';

  constructor(private http: HttpClient, private authService:AuthService) {}

  ngOnInit(): void {
    this.authService.checkToken();
    this.loadData();
  }

  getHeaders() {
    const token = localStorage.getItem('token');
    return { headers: new HttpHeaders({ Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' }) };
  }

  loadData(): void {
    this.http.get<any[]>(`${this.apiBaseUrl}/Hospital`, this.getHeaders()).subscribe(data => {
      this.hospitals = data;
    });

    this.http.get<any[]>(`${this.apiBaseUrl}/HospitalAsset`, this.getHeaders()).subscribe(data => {
      this.assets = data;
    });

    this.http.get<any[]>(`${this.apiBaseUrl}/Medicine`, this.getHeaders()).subscribe(data => {
      this.medicines = data;
    });
  }

  openModal(type: string, item: any = null) {
    this.entityType = type;
    this.selectedItem = item ? { ...item } : {}; // Clone object for editing
    this.showModal = true;
  }

  saveData(form: any): void {
    if (!form.valid) return;
  
    let apiUrl = `${this.apiBaseUrl}/${this.entityType}`;
    let body = JSON.stringify(this.selectedItem);
  
    // HOSPITAL ID FOR ASSET AND MEDICINE APIs
    if (this.entityType === 'HospitalAsset') {
      // Add Asset to Hospital (POST API)
      const hospitalId = this.selectedItem.hospitalId; // Ensure hospitalId is set
      apiUrl = `${this.apiBaseUrl}/HospitalAsset?hospitalId=${hospitalId}`;
    } else if (this.entityType === 'Medicine') {
      // Add Medicine to Hospital (POST API)
      const hospitalId = this.selectedItem.hospitalId; // Ensure hospitalId is set
      apiUrl = `${this.apiBaseUrl}/Medicine?hospitalId=${hospitalId}`;
      // Ensure hospitalId is included in the request body
      const newMedicine = {
        name: this.selectedItem.name,
        stock: this.selectedItem.stock,
        price: this.selectedItem.price,
        hospitalId: hospitalId
      };
      body = JSON.stringify(newMedicine);
    }
  
    if (this.selectedItem.id) {
      // Edit (PUT API)
      if (this.entityType === 'HospitalAsset') {
        // Only update name and quantity for assets
        const updateBody = JSON.stringify({
          name: this.selectedItem.name,
          quantity: this.selectedItem.quantity
        });
        this.http.put(`${this.apiBaseUrl}/HospitalAsset/${this.selectedItem.id}`, updateBody, this.getHeaders()).subscribe(
          () => {
            alert(`${this.entityType} updated successfully!`);
            this.showModal = false;
            this.loadData();
          },
          error => {
            console.error('Error:', error);
          }
        );
      } else if (this.entityType === 'Medicine') {
        // Only update name, stock, and price for medicines
        const updateBody = JSON.stringify({
          name: this.selectedItem.name,
          stock: this.selectedItem.stock,
          price: this.selectedItem.price
        });
        this.http.put(`${this.apiBaseUrl}/Medicine/${this.selectedItem.id}`, updateBody, this.getHeaders()).subscribe(
          () => {
            alert(`${this.entityType} updated successfully!`);
            this.showModal = false;
            this.loadData();
          },
          error => {
            console.error('Error:', error);
          }
        );
      } else {
        this.http.put(`${apiUrl}/${this.selectedItem.id}`, body, this.getHeaders()).subscribe(
          () => {
            alert(`${this.entityType} updated successfully!`);
            this.showModal = false;
            this.loadData();
          },
          error => {
            console.error('Error:', error);
          }
        );
      }
    } else {
      // Add (POST API)
      if (this.entityType === 'Medicine') {
        // Add new medicine
        this.http.post(apiUrl, body, this.getHeaders()).subscribe(
          () => {
            alert(`${this.entityType} added successfully!`);
            this.showModal = false;
            this.loadData();
          },
          error => {
            console.error('Error:', error);
          }
        );
      } else {
        this.http.post(apiUrl, body, this.getHeaders()).subscribe(
          () => {
            alert(`${this.entityType} added successfully!`);
            this.showModal = false;
            this.loadData();
          },
          error => {
            console.error('Error:', error);
          }
        );
      }
    }
  }

  // DELETE API
  deleteData(type: string, id: number) {
    if (!confirm(`Are you sure you want to delete this ${type}?`)) return;

    let apiUrl = `${this.apiBaseUrl}/${type}/${id}`;
    this.http.delete(apiUrl, { ...this.getHeaders(), responseType: 'text' as 'json' }).subscribe(
      response => {
        console.log('Delete response:', response); // Log the response
        alert(`${type} deleted successfully!`);
        this.loadData(); // Ensure data is reloaded after deletion
      },
      error => {
        console.error('Error:', error);
        alert(`Failed to delete ${type}. Please try again.`);
      }
    );
  }

   // Logout function
   logout(): void {
    this.authService.logout();
    alert('You have been logged out.');
  } 
}

