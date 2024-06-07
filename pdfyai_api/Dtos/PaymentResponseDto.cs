namespace pdfyai_api.Models
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public string PaypalId { get; set; } = null!;
        public string PaymentDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;

    }
}