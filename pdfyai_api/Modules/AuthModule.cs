using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdfyai_api.Data;
using pdfyai_api.Dtos.User;
using pdfyai_api.Models;
using pdfyai_api.Utils;

namespace pdfyai_api.Modules
{
    public static class AuthModule
    {
        public static IEndpointRouteBuilder AddAuthModule(this IEndpointRouteBuilder routeBuilder, IConfiguration configuration)
        {

            routeBuilder.MapPost("/api/auth/google-signup", async ([FromBody] string credential, UserManager<ApplicationUser> userManager, ApplicationDbContext context, TokenManager tokenManager, HttpContext httpContext) =>
            {

                try
                {
                    if (credential is null)
                        return Results.BadRequest();


                    var settings = new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new List<string>() { configuration.GetSection("Google:ClientId").Value! }
                    };

                    GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);


                    if (payload is null)
                        return Results.BadRequest("");


                    ApplicationUser? user = await userManager.Users
                    .Where(u => u.Email == payload.Email).FirstOrDefaultAsync();

                    if (user is null)
                    {
                        ApplicationUser newUser = new()
                        {
                            Email = payload.Email,
                            UserName = $"{payload.GivenName}_{payload.IssuedAtTimeSeconds}",
                            Fullname = payload.Name,
                            Picture = payload.Picture,
                            EmailConfirmed = true,
                        };

                        var res = await userManager.CreateAsync(newUser);

                        if (res.Succeeded)
                        {
                            tokenManager.GenerateToken(newUser, httpContext);
                            return Results.Ok();
                        }
                        else
                            return Results.BadRequest("");
                    }
                    else
                    {
                        tokenManager.GenerateToken(user, httpContext);
                        return Results.Ok();
                    }



                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }

            }).AllowAnonymous();




            routeBuilder.MapGet("/api/user-info", (HttpContext context, ApplicationDbContext dbContext, AuthUtils authUtils) =>
            {

                ApplicationUser? applicationUser = authUtils.GetLoggedUser(httpContext: context);

                if (applicationUser is null)
                    return Results.NotFound("User not found");

                IEnumerable<PaymentResponseDto> payments = dbContext.Payments
                .Where(p => p.UserId == applicationUser.Id)
                .Select(p => new PaymentResponseDto()
                {
                    Id = p.Id,
                    PaymentDate = p.PaymentDate.ToString("dd/MM/yyyy"),
                    EndDate = p.EndDate.ToString("dd/MM/yyyy") ?? DateTime.UtcNow.ToString("dd/MM/yyyy"),
                    PaypalId = p.PaypalId
                }).AsNoTracking();

                var userResponseDto = new UserResponseDto()
                {
                    Email = applicationUser.Email ?? "",
                    Username = applicationUser.Fullname,
                    Picture = applicationUser.Picture,
                    RemainingDocuments = applicationUser.NumberOfDocuments,
                    RemainingQuestions = applicationUser.NumberOfQuestions,
                    Payments = payments
                };

                return Results.Ok(userResponseDto);

            }).RequireAuthorization();



            routeBuilder.MapPost("/api/signout", async (HttpContext context) =>
            {

                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Results.Ok();

            }).RequireAuthorization();

            return routeBuilder;
        }
    }
}