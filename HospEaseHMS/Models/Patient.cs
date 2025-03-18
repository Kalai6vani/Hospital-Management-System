namespace HospEaseHMS.Models
{
    public class Patient:User
    {
        //one-to-many relationship with MedicalRecord
        //one-to-many relationship with Appointment
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
