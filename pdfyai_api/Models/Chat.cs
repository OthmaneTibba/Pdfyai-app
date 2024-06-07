using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfyai_api.Models
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        [NotMapped]
        public ApplicationUser? User { get; set; }
        public Guid? DocumentId { get; set; }
        [NotMapped]
        public Document? Document { get; set; }
        public DateTime CreatedOn { get; set; }
        [NotMapped]
        public List<Message> Messages { get; set; } = new();

    }
}