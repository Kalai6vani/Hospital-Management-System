using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class Billing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime BillingDate { get; set; } = DateTime.Now;

        public Patient Patient { get; set; }

        public ICollection<BillingDetail> BillingDetails { get; set; }
    }
}
