using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class Medicine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int HospitalId { get; set; }

        public Hospital Hospital { get; set; }
    }
}
