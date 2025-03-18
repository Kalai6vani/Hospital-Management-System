using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class BillingDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BillingId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public Billing Billing { get; set; }

        public Medicine Medicine { get; set; }
    }
}
