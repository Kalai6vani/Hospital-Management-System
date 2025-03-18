using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.DTOs
{
    public class MedicalRecordDTO
    {
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
    }
}
