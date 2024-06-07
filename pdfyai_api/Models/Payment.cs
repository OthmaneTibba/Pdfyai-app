using System.ComponentModel.DataAnnotations;

namespace pdfyai_api.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        public string PaypalId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public DateTime PaymentDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}