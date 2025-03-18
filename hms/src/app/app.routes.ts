import { Routes } from '@angular/router';

//add imports
import {HomeComponent} from './home/home.component';
import {LoginComponent} from './auth/login/login.component';
import {RegisterComponent} from './auth/register/register.component';
import {AdminComponent} from './dashboard/admin/admin.component';
import {PatientComponent} from './dashboard/patient/patient.component';
import {DoctorComponent} from './dashboard/doctor/doctor.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'admin-dashboard', component: AdminComponent},
    {path: 'patient-dashboard', component: PatientComponent},
    {path: 'doctor-dashboard', component: DoctorComponent},
    {path: '**', redirectTo: '', pathMatch: 'full'}  //redirects unknown routes to home
];
