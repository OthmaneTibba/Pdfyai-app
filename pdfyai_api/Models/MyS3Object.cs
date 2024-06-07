
namespace pdfyai_api.Models
{
    public class MyS3Object
    {
        public string Name { get; set; } = null!;
        public MemoryStream InputStream { get; set; } = null!;
        public string BucketName { get; set; } = string.Empty;
    }
}