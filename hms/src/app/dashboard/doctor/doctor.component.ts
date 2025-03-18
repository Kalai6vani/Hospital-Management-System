import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-doctor',
  imports: [FormsModule, CommonModule],
  templateUrl: './doctor.component.html',
  styleUrls: ['./doctor.component.css']
})

export class DoctorComponent implements OnInit {
  doctorId: number = parseInt(localStorage.getItem('userId') || '0', 10);
  doctor: any = { name: '', email: '' };
  scheduledAppointments: any[] = [];
  pastAppointments: any[] = [];
  selectedAppointment: any = null;
  diagnosis: string = '';
  prescription: string = '';
  patientName: string = ''; // Add patientName field
  patientRecords: any[] = []; // Add patientRecords field
  apiBaseUrl = 'https://localhost:7003/api'; // Replace with your API URL
  showRecordModal: boolean = false;

  constructor(private http: HttpClient, private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.authService.checkToken();
    this.getDoctorDetails();
    this.getAppointments();
  }

  getHeaders() {
    const token = localStorage.getItem('token');
    return { headers: new HttpHeaders({ Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' }) };
  }

  // Fetch Doctor Details
  getDoctorDetails() {
    this.http.get(`${this.apiBaseUrl}/Auth/${this.doctorId}`, this.getHeaders())
      .subscribe(data => {
        this.doctor = data;
      }, error => {
        console.error('Error fetching doctor details:', error);
      });
  }

  // Fetch Appointments
  getAppointments() {
    this.http.get<any[]>(`${this.apiBaseUrl}/Appointment/doctor/${this.doctorId}`, this.getHeaders())
      .subscribe(data => {
        this.scheduledAppointments = data.filter(appointment => appointment.status === 'Booked');
        this.pastAppointments = data.filter(appointment => appointment.status !== 'Booked');
      }, error => {
        console.error('Error fetching appointments:', error);
      });
  }

  // Mark Appointment as Completed
  completeAppointment(appointmentId: number) {
    this.http.put(`${this.apiBaseUrl}/Appointment/confirm/${appointmentId}`, {}, this.getHeaders())
      .subscribe({
        next: (response) => {
          alert('Appointment marked as completed!');
          this.getAppointments(); // Refresh appointments list
        },
        error: (err) => {
          alert('Appointment marked as completed!');
          this.getAppointments(); // Refresh appointments list
        }
      });
  }

  // Provide Medical Record
  provideRecord() {
    if (!this.diagnosis.trim() || !this.prescription.trim()) {
      alert('Please fill in both diagnosis and prescription.');
      return;
    }

    const recordData = {
      patientId: this.selectedAppointment.patientId,
      doctorId: this.doctorId,
      diagnosis: this.diagnosis,
      prescription: this.prescription,
      appointmentId: this.selectedAppointment.id // Include appointmentId
    };

    this.http.post(`${this.apiBaseUrl}/MedicalRecord`, recordData, this.getHeaders())
      .subscribe({
        next: (response) => {
          alert('Medical record provided successfully!');
        },
        error: (err) => {
          console.error('Error providing medical record:', err);
          alert('An error occurred while providing the medical record.');
        }
      });
  }

  openRecordModal(appointment: any) {
    this.selectedAppointment = appointment;
    this.showRecordModal = true;
  }

  // Close Record Modal
  closeRecordModal() {
    this.selectedAppointment = null;
    this.diagnosis = '';
    this.prescription = '';
    this.showRecordModal = false;
  }

  // Logout
  logout() {
    this.authService.logout();
    alert('You have been logged out.');
  }
}
