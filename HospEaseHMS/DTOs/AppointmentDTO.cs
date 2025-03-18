using System.Text.Json.Serialization;

namespace HospEaseHMS.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int PatientId {  get; set; }

        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
    }
}
