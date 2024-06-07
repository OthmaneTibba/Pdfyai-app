namespace pdfyai_api.Dtos
{
    public class UserDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Picture { get; set; } = null!;
        public dynamic Token { get; set; } = null!;
    }
}