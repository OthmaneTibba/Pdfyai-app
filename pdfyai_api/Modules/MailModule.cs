

using pdfyai_api.Services.IServices;

namespace pdfyai_api.Modules
{
    public static class MailModule
    {
        public static IEndpointRouteBuilder AddEmailModule(this IEndpointRouteBuilder routeBuilder)
        {


            routeBuilder.MapPost("/api/contact-us", async (IEmailService emailService) =>
            {

                await emailService.SendEmailAsync("put you email here", "contact", "i need help");

                return Results.Ok();

            }).RequireAuthorization();


            return routeBuilder;
        }
    }
}