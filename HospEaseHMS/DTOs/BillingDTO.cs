namespace HospEaseHMS.DTOs
{
    public class BillingDTO
    {
        public int PatientId {  get; set; }
        public List<BillingDetailDTO> BillingDetails { get; set; }
    }
}
