using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pdfyai_api.Enums;

namespace pdfyai_api.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string Content { get; set; } = null!;
        public string Role { get; set; } = ChatRole.USER.ToString();
        public DateTime CreatedOn { get; set; }
        public Guid? ChatId { get; set; }
        [NotMapped]
        public Chat? Chat { get; set; }
    }
}