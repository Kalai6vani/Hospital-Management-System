using HospEaseHMS.Data;
using HospEaseHMS.DTOs;
using HospEaseHMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospEaseHMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public MedicalRecordController(HospitalDbContext context)
        {
            _context = context;
        }

        // create medical record - doctor
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] MedicalRecordDTO medicalRecordDto)
        {
            var patient = await _context.Patients.FindAsync(medicalRecordDto.PatientId);
            var doctor = await _context.Doctors.FindAsync(medicalRecordDto.DoctorId);
            var appointment = await _context.Appointments.FindAsync(medicalRecordDto.AppointmentId);

            if (patient == null || doctor == null || appointment == null)
            {
                return BadRequest("Invalid PatientId, DoctorId, or AppointmentId.");
            }

            var medicalRecord = new MedicalRecord
            {
                Diagnosis = medicalRecordDto.Diagnosis,
                Prescription = medicalRecordDto.Prescription,
                PatientId = medicalRecordDto.PatientId,
                DoctorId = medicalRecordDto.DoctorId,
                AppointmentId = medicalRecordDto.AppointmentId
            };

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();
            var response = new
            {
                Id = medicalRecord.Id,
                medicalRecord.Diagnosis,
                medicalRecord.Prescription,
                medicalRecord.PatientId,
                medicalRecord.DoctorId,
                medicalRecord.AppointmentId
            };
            return CreatedAtAction(nameof(GetMedicalRecords), new { id = medicalRecord.Id }, response);
        }
        // get medical record by id - patient/doctor
        [Authorize(Roles = "Patient, Doctor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalRecords(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Include(mr => mr.Appointment)
                .FirstOrDefaultAsync(mr => mr.Id == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            var response = new
            {
                Id = medicalRecord.Id,
                medicalRecord.Diagnosis,
                medicalRecord.Prescription,
                medicalRecord.PatientId,
                medicalRecord.DoctorId,
                medicalRecord.AppointmentId,
                PatientName = medicalRecord.Patient.Name,
                DoctorName = medicalRecord.Doctor.Name,
                AppointmentDate = medicalRecord.Appointment.AppointmentDate
            };

            return Ok(response);
        }
    }
}
