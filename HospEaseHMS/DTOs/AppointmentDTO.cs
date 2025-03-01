using System.Text.Json.Serialization;

namespace HospEaseHMS.DTOs
{
    public class AppointmentDTO
    {
        public int PatientId {  get; set; }
        public int DoctorId {  get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
