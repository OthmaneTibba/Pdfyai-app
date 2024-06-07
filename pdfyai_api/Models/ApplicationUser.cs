using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using pdfyai_api.Contstans;
using pdfyai_api.Enums;

namespace pdfyai_api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Picture { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public int NumberOfDocuments { get; set; } = AppContstants.FREE_MAX_DOCUMENTS;
        public int NumberOfQuestions { get; set; } = AppContstants.FREE_MAX_QUESTIONS;
    }
}