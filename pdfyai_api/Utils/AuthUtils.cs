using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using pdfyai_api.Models;

namespace pdfyai_api.Utils
{
    public class AuthUtils
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public AuthUtils(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser? GetLoggedUser(HttpContext httpContext)
        {
            ClaimsIdentity? identity = httpContext.User.Identity as ClaimsIdentity;

            if (identity is null)
                return null;

            Claim? userEmail = identity.Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();

            if (userEmail is null)
                return null;

            ApplicationUser? user = _userManager.Users
            .Where(u => u.Email == userEmail.Value).FirstOrDefault();

            if (user is null)
                return null;
            return user;
        }



    }
}