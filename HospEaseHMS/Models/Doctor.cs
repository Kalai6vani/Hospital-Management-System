using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class Doctor:User
    {
        [Required]
        public string Specialization { get; set; }
    }
}
