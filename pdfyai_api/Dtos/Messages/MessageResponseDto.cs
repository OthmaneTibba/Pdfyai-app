// this class is dto to reprense message when getting a chat  by id 
namespace pdfyai_api.Dtos.Messages
{
    public class MessageResponseDto
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}