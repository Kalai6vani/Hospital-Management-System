using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class Hospital
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        //one-to-many relationship with HospitalAsset
        //one-to-many relationship with Medicine
        public ICollection<HospitalAsset> Assets { get; set; }
        public ICollection<Medicine> Medicines { get; set; }
    }
}
