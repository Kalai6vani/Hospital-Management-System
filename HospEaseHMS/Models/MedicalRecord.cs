using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Appointment Appointment { get; set; }
    }
}
