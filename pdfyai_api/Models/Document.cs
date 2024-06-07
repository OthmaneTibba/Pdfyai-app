using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfyai_api.Models
{
    [Table("Documents")]
    public class Document
    {
        [Key]
        public Guid Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentUniqueName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string UserId { get; set; } = null!;
        [NotMapped]
        public ApplicationUser? User { get; set; }
    }
}