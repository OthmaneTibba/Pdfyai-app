using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using pdfyai_api.Contstans;
using pdfyai_api.Data;
using pdfyai_api.Dtos;
using pdfyai_api.Models;
using pdfyai_api.Services.IServices;
using pdfyai_api.Utils;

namespace pdfyai_api.Modules
{
    public static class PaymentModule
    {
        public static IEndpointRouteBuilder AddPaymentModule(this IEndpointRouteBuilder routeBuilder, IConfiguration configuration)
        {

            routeBuilder.MapPost("/api/paypal-webhook", async (ApplicationDbContext dbContext, ConfirmPaymentDto confirmPaymentDto, AuthUtils authUtils, HttpContext context, IEmailService emailService) =>
            {

                ApplicationUser? applicationUser = authUtils.GetLoggedUser(context);

                if (applicationUser is null)
                {
                    return Results.Unauthorized();
                }

                var clientId = configuration["Paypal:ClientId"];
                var secretId = configuration["Paypal:SecretId"];
                var env = new LiveEnvironment(clientId, secretId);
                var client = new PayPalHttpClient(env);

                Payment? isPaymentIdExist = dbContext
                .Payments
                .Where(e => e.PaypalId == confirmPaymentDto.PaymentId)
                .FirstOrDefault();

                if (isPaymentIdExist is not null)
                    return Results.BadRequest("Cannot confirm your payment");

                // checki wach deja kayna had order id f bd 3ndi wel la 


                var request = new OrdersGetRequest("9RB936794L367432D");
                var response = await client.Execute(request);

                var result = response.Result<Order>();

                if (result.Status == "COMPLETED")
                {

                    Payment payment = new()
                    {
                        PaypalId = confirmPaymentDto.PaymentId,
                        User = applicationUser,
                        UserId = applicationUser.Id,
                        PaymentDate = DateTime.UtcNow
                    };


                    // Update user information
                    applicationUser.NumberOfDocuments += AppContstants.PREMIUM_MAX_DOCUMENTS;
                    applicationUser.NumberOfQuestions += AppContstants.PREMIUM_MAX_QUESTIONS;


                    dbContext.Payments.Add(payment);

                    await dbContext.SaveChangesAsync();
                    // Transaction is valid

                    await emailService.SendEmailAsync(applicationUser.Email!, "You for Your Subscription Purchase", "We hope this email finds you well. Thank you for choosing PdfyAI We are delighted to inform you that your subscription purchase has been successfully processed, and you are now set to enjoy the benefits of our premium services.");
                    // 

                    return Results.Ok("Transaction valid");
                }
                else
                {
                    return Results.BadRequest("Transaction not valid");
                }


            }).RequireAuthorization();


            return routeBuilder;
        }
    }
}