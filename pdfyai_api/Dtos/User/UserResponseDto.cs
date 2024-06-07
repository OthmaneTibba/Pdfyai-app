using pdfyai_api.Models;

namespace pdfyai_api.Dtos.User
{
    public class UserResponseDto
    {
        public string Email { get; set; } = null!;
        public string Username { get; set; } = string.Empty;
        public string Picture { get; set; } = null!;

        public IEnumerable<PaymentResponseDto> Payments { get; set; } = null!;
        public int RemainingDocuments { get; set; }
        public int RemainingQuestions { get; set; }
    }
}