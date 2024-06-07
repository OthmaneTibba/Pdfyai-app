using pdfyai_api.Dtos.Messages;
using pdfyai_api.Models;

namespace pdfyai_api.Dtos.Chat
{
    public class GetChatResponseDto
    {
        public Guid ChatId { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentUniqueName { get; set; } = string.Empty;
        public List<MessageResponseDto> Messages { get; set; } = new();
    }
}