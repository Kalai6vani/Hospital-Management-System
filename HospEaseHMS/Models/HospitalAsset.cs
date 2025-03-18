using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class HospitalAsset
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int HospitalId { get; set; }

        public Hospital Hospital { get; set; }
    }
}
