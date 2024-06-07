

using System.Security.Claims;

namespace pdfyai_api.Midls
{
    public class AuthMidl : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                ClaimsIdentity? identity = context.User.Identity as ClaimsIdentity;

                if (identity is null)
                {
                    context.Response.StatusCode = 401;
                    return;
                }


                Claim? userEmail = identity.Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();

                if (userEmail is null)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                await next(context);

            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}