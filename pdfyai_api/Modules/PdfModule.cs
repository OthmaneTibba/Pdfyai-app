using System.Reactive.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdfyai_api.Contstans;
using pdfyai_api.Data;
using pdfyai_api.Dtos;
using pdfyai_api.Enums;
using pdfyai_api.Models;
using pdfyai_api.Services.IServices;
using pdfyai_api.Utils;
using Pinecone;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace pdfyai.Modules
{
    public static class PdfModule
    {


        public static IEndpointRouteBuilder AddPdfModule(this IEndpointRouteBuilder routes, IConfiguration configuration)
        {

            // END POINT TO UPLOAD FILE TO S3 AND SAVE THE DOCULENT TO DATABASE  
            routes.MapPost("/api/upload", async (IFormFile file, IS3Service s3Service, ApplicationDbContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager, PdfAuthils pdfAuthils) =>
            {

                var extension = Path.GetExtension(file.FileName);




                if (extension != ".pdf")
                    return Results.BadRequest("Only pdf files are accepted");

                if (!pdfAuthils.CanReadDocuments(file))
                    return Results.BadRequest("Cannot read this file");



                ClaimsIdentity? identity = httpContext.User.Identity as ClaimsIdentity;
                if (identity is null)
                    return Results.Unauthorized();

                Claim? userEmail = identity.Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
                if (userEmail is null)
                    return Results.BadRequest("Cannot verify your identity");


                ApplicationUser? user = await userManager.Users.Where(u => u.Email == userEmail.Value).FirstOrDefaultAsync();

                if (user is null)
                {
                    return Results.Unauthorized();
                }

                if (user.NumberOfDocuments <= 0)
                    return Results.BadRequest("You have consumed all your documents");

                long tenMegabytesInBytes = 10 * 1024 * 1024;
                long twentyMegabutesInButes = 2 * tenMegabytesInBytes;




                if (user.NumberOfDocuments == 1)
                {
                    if (file.Length > tenMegabytesInBytes)
                        return Results.BadRequest("the file is large");
                }
                else if (user.NumberOfDocuments >= 2)
                {
                    if (file.Length > twentyMegabutesInButes)
                        return Results.BadRequest("the file is large");
                }


                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);

                    var uniqueName = $"{Guid.NewGuid().ToString()}{file.FileName}";

                    var document = new Document()
                    {
                        DocumentName = file.FileName,
                        DocumentUniqueName = uniqueName,
                        CreatedOn = DateTime.UtcNow,
                        UserId = user.Id,
                        User = user
                    };
                    //Reduce the number of document that can a user upload


                    context.Documents.Add(document);

                    var results = await s3Service.UploadFileAsync(
                     new MyS3Object()
                     {
                         Name = uniqueName,
                         BucketName = AppContstants.BUCKET_NAME,
                         InputStream = ms
                     }
                      );

                    if (results)
                    {
                        user.NumberOfDocuments -= 1;
                        await context.SaveChangesAsync();
                        return Results.Ok(document);
                    }

                    return Results.BadRequest();

                }


            })
            .DisableAntiforgery()
            .RequireAuthorization();

            routes.MapGet("/api/download/{fileName}", async (string fileName, IS3Service s3Service) =>
                {
                    var response = await s3Service.DownloadFileAsync(fileName);

                    return Results.File(response.ResponseStream, "application/pdf", fileName);


                }).RequireAuthorization();


            routes.MapDelete("/api/delete/{docId}", async (Guid docId, AuthUtils authUtils, ApplicationDbContext dbContext, HttpContext httpContext, IS3Service s3Service, PineconeClient pinecone) =>
            {

                ApplicationUser? user = authUtils.GetLoggedUser(httpContext);

                if (user is null)
                {
                    return Results.BadRequest("User not found please log in or try again");
                }

                Document? document = await dbContext.Documents.Include(d => d.User)
                .Where(d => d.UserId == user.Id && d.Id == docId)
                .FirstOrDefaultAsync();





                if (document is null)
                    return Results.NotFound("Could not found the document");



                var res = await s3Service.DeleteFileAsync(AppContstants.BUCKET_NAME, document.DocumentUniqueName);


                if (res)
                {



                    Chat? chat = await dbContext.Chats
                    .Where(c => c.Document!.Id == document.Id)
                    .FirstOrDefaultAsync();

                    if (chat is null)
                        return Results.BadRequest("Cannot find chat");

                    List<Message> messages = await dbContext.Messages
                    .Where(m => m.ChatId == chat.Id)
                    .ToListAsync();


                    var indexName = AppContstants.PINCONE_INDEX_NAME;
                    var index = await pinecone.GetIndex(indexName);
                    await index.DeleteAll(chat.Id.ToString());
                    dbContext.Messages.RemoveRange(messages);
                    dbContext.Chats.Remove(chat!);
                    dbContext.Documents.Remove(document);
                    await dbContext.SaveChangesAsync();

                    return Results.Ok();
                }

                return Results.BadRequest("Could not delete the document");

            }).RequireAuthorization();


            routes.MapPost("/api/generate", ([FromBody] GeneratePdfDto generatePdfDto, HttpContext httpContext, AuthUtils authUtils) =>
            {
                try
                {


                    ApplicationUser? applicationUser = authUtils.GetLoggedUser(httpContext);

                    if (applicationUser is null)
                        return Results.Unauthorized();


                    var pdf = new PdfSharpCore.Pdf.PdfDocument();
                    PdfGenerator.AddPdfPages(pdf, generatePdfDto.HtmlContent, PdfSharpCore.PageSize.A4);

                    byte[]? data;

                    using (var ms = new MemoryStream())
                    {
                        pdf.Save(ms);
                        data = ms.ToArray();
                    }

                    return Results.File(data, "application/pdf", "test.pdf");
                }
                catch (Exception e)
                {
                    return Results.BadRequest("Content cannot be empty");
                }
            }).RequireAuthorization();

            return routes;
        }


    }
}