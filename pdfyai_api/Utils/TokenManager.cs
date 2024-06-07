using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using pdfyai_api.Models;

namespace pdfyai_api.Utils
{
    public class TokenManager
    {

        private readonly IConfiguration _configuration;

        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void GenerateToken(ApplicationUser user, HttpContext httpContext)
        {
            var claims = new Claim[]{
                   new Claim(ClaimTypes.NameIdentifier , user.Id),
                    new Claim(ClaimTypes.Name , user.UserName ?? ""),
                    new Claim(ClaimTypes.Email , user.Email ?? ""),
                    new Claim("Picture" , user.Picture),
                    new Claim("Fullname" , user.Fullname)
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);


            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = credential,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _configuration["JWT:Issuer"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var decodedToken = tokenHandler.WriteToken(token);


            httpContext.Response.Cookies.Append("token", decodedToken, new CookieOptions()
            {
                Expires = DateTime.Now.AddHours(1),
                Secure = true,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}