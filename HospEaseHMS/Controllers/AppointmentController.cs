using HospEaseHMS.Data;
using HospEaseHMS.DTOs;
using HospEaseHMS.Models;
using HospEaseHMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospEaseHMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly EmailService _emailService;

        public AppointmentController(HospitalDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        //booking appointment - patient
        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<IActionResult> BookAppointment(AppointmentDTO model)
        {

            var doctor = await _context.Doctors.FindAsync(model.DoctorId);
            var patient = await _context.Patients.FindAsync(model.PatientId);
            if (doctor == null || patient == null) return NotFound("Invalid doctor or patient.");

            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                Status = "Booked"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            //sending email - queue
            string subject = "Appointment Booked";
            string body = $"Your appointment with Dr. {doctor.Name} has been booked for {model.AppointmentDate}.";  
            await _emailService.SendEmailAsync(patient.Email, subject, body);
            await _emailService.SendEmailAsync(doctor.Email, subject, $"You have a new appointment with {patient.Name} on {model.AppointmentDate}");
            return Ok("Appointment booked.");
        }

        //get all appointments - admin
        [Authorize(Roles = "Admin, Patient, Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments.ToListAsync();
            return Ok(appointments);
        }

        //confirm appointment - doctor
        [Authorize(Roles = "Doctor")]
        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = "Completed";
            await _context.SaveChangesAsync();
            return Ok("Appointment marked as completed.");
        }

        /*  [HttpGet("patient/{id}")]
          public async Task<IActionResult> GetAppointmentsForPatient(int id)
          {
              var appointments = await _context.Appointments
                  .Where(a => a.PatientId == id)
                  .Include(a => a.Doctor)
                  .ToListAsync();

              return Ok(appointments);
          }

          [HttpGet("doctor/{id}")]
          public async Task<IActionResult> GetAppointmentsForDoctor(int id)
          {
              var appointments = await _context.Appointments
                  .Where(a => a.DoctorId == id)
                  .Include(a => a.Patient)
                  .ToListAsync();

              return Ok(appointments);
          }*/

        //get appointments for patient - patient
        [Authorize(Roles = "Patient")]
        [HttpGet("patient/{id}")]
        public async Task<IActionResult> GetAppointmentsForPatient(int id)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == id)
                .Include(a => a.Doctor)
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.Name,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(appointments);
        }

        //get appointments for doctor - doctor
        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetAppointmentsForDoctor(int id)
        {
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == id)
                .Include(a => a.Patient)
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.Name,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(appointments);
        }
    }
}

