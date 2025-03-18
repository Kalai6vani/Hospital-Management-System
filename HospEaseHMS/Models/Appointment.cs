using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending"; // Pending, Completed

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
