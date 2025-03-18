import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-patient',
  imports: [FormsModule, CommonModule],
  templateUrl: './patient.component.html',
  styleUrls: ['./patient.component.css']
})
export class PatientComponent implements OnInit {
  patientId: number = parseInt(localStorage.getItem('userId') || '0', 10);
  patient: any = { name: '', email: '' };
  medicines: any[] = [];
  doctors: any[] = [];
  appointments: any[] = [];
  selectedMedicine: any = null;
  quantity: number = 1;
  chatbotQuery: string = '';
  chatbotResponse: string = '';
  appointmentDetails = { date: '', doctorId: 0 }; // Initialize doctorId as a number
  billAmount: number = 0;
  medicalRecords: any[] = [];
  apiBaseUrl = 'https://localhost:7003/api'; // Replace with your API URL
  showBillModal: boolean = false;
  showAppointmentModal: boolean = false;
  selectedDoctor: any;

  constructor(private http: HttpClient, private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.authService.checkToken();
    this.getPatientDetails();
    this.getMedicines();
    this.getDoctors();
    this.getMedicalRecords();
    this.getAppointments();
  }

  getHeaders() {
    const token = localStorage.getItem('token');
    return { headers: new HttpHeaders({ Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' }) };
  }

  // Fetch Patient Details
  getPatientDetails() {
    this.http.get(`${this.apiBaseUrl}/Auth/${this.patientId}`, this.getHeaders())
      .subscribe(data => {
        this.patient = data;
      }, error => {
        console.error('Error fetching patient details:', error);
      });
  }

  // Fetch List of Medicines
  getMedicines() {
    this.http.get<any[]>(`${this.apiBaseUrl}/Medicine`, this.getHeaders())
      .subscribe(data => {
        this.medicines = data;
      }, error => {
        console.error('Error fetching medicines:', error);
      });
  }

  // Fetch List of Doctors
  getDoctors() {
    this.http.get<any[]>(`${this.apiBaseUrl}/Auth?role=doctor`, this.getHeaders())
      .subscribe(data => {
        this.doctors = data;
      }, error => {
        console.error('Error fetching doctors:', error);
      });
  }

  // Fetch Past Medical Records
  getMedicalRecords() {
    this.http.get<any[]>(`${this.apiBaseUrl}/MedicalRecord/GetMedicalRecordsByPatientId/${this.patientId}`, this.getHeaders())
      .subscribe(data => {
        this.medicalRecords = data;
      }, error => {
        console.error('Error fetching medical records:', error);
      });
  }

  // Update Price When Medicine is Selected
  onMedicineSelect() {
    if (this.selectedMedicine) {
      if (this.quantity > this.selectedMedicine.stock) {
        alert(`The available stock is ${this.selectedMedicine.stock}. Please adjust the quantity.`);
        this.quantity = this.selectedMedicine.stock;
      }
      this.billAmount = this.selectedMedicine.price * this.quantity;
    }
  }

  // Buy Medicine
  buyMedicine() {
    if (!this.selectedMedicine || this.quantity <= 0 || this.quantity > this.selectedMedicine.stock) {
      alert('Invalid medicine selection or stock exceeded!');
      return;
    }

    const purchaseUrl = `${this.apiBaseUrl}/Medicine/purchase?patientId=${this.patientId}&medicineId=${this.selectedMedicine.id}&quantity=${this.quantity}`;

    this.http.post(purchaseUrl, {}, this.getHeaders())
      .subscribe({
        next: (response) => {
          alert('Medicine purchased successfully!');
          this.getMedicines();  // Refresh medicine stock
          this.showBillModal = true; // Show the bill modal
        },
        error: (err) => {
          console.error('Error purchasing medicine:', err);
          if (err.status === 404) {
            alert('The requested resource was not found.');
          } else {
            alert('An error occurred while purchasing the medicine.');
          }
        }
      });
  }

  // Close Bill Modal
  closeBillModal() {
    this.showBillModal = false;
  }

  // Fetch Appointments
  getAppointments() {
    this.http.get<any[]>(`${this.apiBaseUrl}/Appointment/patient/${this.patientId}`, this.getHeaders())
      .subscribe(data => {
        this.appointments = data.filter(appointment => appointment.status === 'Booked'); // Filter appointments with status "Booked"
      }, error => {
        console.error('Error fetching appointments:', error);
      });
  }

  // Book Appointment
bookAppointment() {
  if (!this.appointmentDetails.date || !this.appointmentDetails.doctorId) {
      alert('Please fill all appointment details!');
      return;
  }

  const appointmentData = {
      id: 0, // Assuming new appointment, set id to 0 or remove if not required
      patientId: this.patientId,
      patientName: this.patient.name, // Assuming patient name is fetched in getPatientDetails
      doctorId: this.appointmentDetails.doctorId,
      doctorName: this.findDoctorById(this.appointmentDetails.doctorId)?.name || '', // Assuming doctor name is fetched in getDoctors
      appointmentDate: this.appointmentDetails.date,
      status: 'Booked' // Set status to 'Booked'
  };

  this.http.post(`${this.apiBaseUrl}/Appointment`, appointmentData, this.getHeaders())
      .subscribe({
          next: (response) => {
              alert('Appointment booked successfully! An email has been sent.');
              this.selectedDoctor = this.findDoctorById(this.appointmentDetails.doctorId);
              this.getAppointments(); // Refresh appointments list
          }, error: (err) => {
            alert('Appointment booked successfully! An email has been sent.');
            this.selectedDoctor = this.findDoctorById(this.appointmentDetails.doctorId);
            this.showAppointmentModal = true;
            this.getAppointments(); 
          }
      });
}
  // Close Appointment Modal
  closeAppointmentModal() {
    this.showAppointmentModal = false;
  }

  // Find Doctor by ID
  findDoctorById(doctorId: number) {
    return this.doctors.find(doctor => doctor.id === doctorId);
  }

  // Chatbot Query Submission
  askChatbot() {
    if (!this.chatbotQuery.trim()) {
      alert('Please enter a query before submitting.');
      return;
    }

    this.http.get(`${this.apiBaseUrl}/ChatBot/${encodeURIComponent(this.chatbotQuery)}`, this.getHeaders())
      .subscribe({
        next: (response: any) => {
          console.log('Chatbot response:', response); // Log the entire response
          if (response && response.result) {
            this.chatbotResponse = response.result;
          } else {
            this.chatbotResponse = 'No response received';
          }
          this.cdr.detectChanges(); // Manually trigger change detection
        },
        error: (err) => {
          console.error('Error with chatbot query:', err);
          this.chatbotResponse = 'An error occurred while fetching the response.';
          this.cdr.detectChanges(); // Manually trigger change detection
        }
      });
  }

  // Logout
  logout() {
    this.authService.logout();
    alert('You have been logged out.');
  }
}