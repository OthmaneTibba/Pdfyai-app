namespace pdfyai_api.Dtos.Documents
{
    public class ChatResponseDto
    {
        public Guid ChatId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public Guid DocumentId { get; set; }
        public string DocumentUniqueName { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string CreatedOn { get; set; } = string.Empty;
    }
}