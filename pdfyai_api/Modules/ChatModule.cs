using System.Text.Json;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
using OpenAI_API.Embedding;
using pdfyai.Utils;
using pdfyai_api.Contstans;
using pdfyai_api.Data;
using pdfyai_api.Dtos;
using pdfyai_api.Dtos.Chat;
using pdfyai_api.Dtos.Documents;
using pdfyai_api.Dtos.Messages;
using pdfyai_api.Enums;
using pdfyai_api.Models;
using pdfyai_api.Services.IServices;
using pdfyai_api.Utils;
using Pinecone;
using UglyToad.PdfPig;

namespace pdfyai_api.Modules
{
    public static class ChatModule
    {
        public static IEndpointRouteBuilder AddChatModule(this IEndpointRouteBuilder routes)
        {


            routes.MapGet("/api/health-check", () =>
            {
                return Results.Ok();
            }).RequireAuthorization();

            routes.MapGet("/api/chats", (HttpContext httpContext) =>
            {

                AuthUtils authUtils = httpContext.RequestServices.GetRequiredService<AuthUtils>();

                ApplicationUser? user = authUtils.GetLoggedUser(httpContext);

                ApplicationDbContext dbContext = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

                if (user is null)
                    return Results.Unauthorized();

                var chats = dbContext.Chats.Include(c => c.User)
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Document)
                .Select(c => new ChatResponseDto()
                {
                    DocumentId = c.Document!.Id,
                    DocumentName = c.Document!.DocumentName,
                    DocumentUniqueName = c.Document.DocumentUniqueName,
                    ChatId = c.Id,
                    CreatedOn = c.Document.CreatedOn.ToString("dd/MM/yyyy")
                })
                .AsNoTracking();

                return Results.Ok(chats);
            }).RequireAuthorization();


            routes.MapPost("/api/create-chat/{documentId}", async (Guid documentId, HttpContext httpContext, AuthUtils authUtils, IS3Service s3Service, ApplicationDbContext context, AppUtils appUtils, OpenAI_API.OpenAIAPI api, PineconeClient pinecone) =>
            {
                // GEt logged in user 
                ApplicationUser? user = authUtils.GetLoggedUser(httpContext);

                if (user is null)
                    return Results.BadRequest();


                if (user.NumberOfDocuments <= 0)
                    return Results.BadRequest("you cannot create more than one document in the free plan");

                Chat? isChatExist = await context.Chats
                .Include(c => c.User)
                .Include(c => c.Document)
                .Where(c => c.UserId == user.Id && c.Document!.Id == documentId)
                .FirstOrDefaultAsync();

                if (isChatExist is not null)
                    return Results.Ok(isChatExist);


                // to check if the document belong to the logged user 
                Document? document = await context.Documents
                .Where(d => d.Id == documentId)
                .Include(d => d.User)
                .Where(d => d.UserId == user.Id)
                .FirstOrDefaultAsync();

                if (document is null)
                {
                    return Results.BadRequest("Document not exist");
                }

                // Download file from s3
                GetObjectResponse? file = await s3Service.DownloadFileAsync(document.DocumentUniqueName);

                if (file is null)
                {
                    return Results.BadRequest();
                }

                string text = "";

                using var streamReader = new StreamReader(file.ResponseStream);

                byte[]? data = null;
                using (var ms = new MemoryStream())
                {
                    await file.ResponseStream.CopyToAsync(ms);
                    data = ms.ToArray();
                }

                if (data == null)
                    return Results.BadRequest("Data is null");

                PdfDocument pdf = PdfDocument.Open(data);


                for (int i = 0; i < pdf.NumberOfPages; i++)
                {
                    var page = pdf.GetPage(i + 1);
                    text += page.Text;
                }

                // Remove non ascii
                text = appUtils.RemoveNoAscii(text);

                // Chunck the doc splited to smalled pieces
                var chunks = appUtils.SplitTextIntoChunks(text, 1000, 200);

                // STRAT CREATING THE EMEBDING TO SAVE THEM TO PINCOCE DB 
                Vector[] vectors = new Vector[chunks.Count];


                // LOOP THROGH THE CHUNKS AND GET EMEBEDING FROM EACH THE ASSIGN THE EMBEDING VALUES TO THE VECTOR ARRAY
                for (int i = 0; i < chunks.Count; i++)
                {

                    var content = chunks[i];

                    EmbeddingResult embeddings = await api.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(content));

                    float[] e = embeddings.Data.First().Embedding;

                    vectors[i] = new Vector
                    {
                        Id = Guid.NewGuid().ToString(),
                        Values = e,
                        Metadata = new MetadataMap
                        {
                            ["text"] = content,
                        }
                    };

                }

                var chat = new Chat()
                {
                    User = user,
                    UserId = user.Id,
                    Document = document,
                    DocumentId = document.Id,

                    CreatedOn = DateTime.UtcNow
                };


                context.Chats.Add(chat);
                await context.SaveChangesAsync();

                var indexName = AppContstants.PINCONE_INDEX_NAME;
                var index = await pinecone.GetIndex(indexName);
                // INSERT ALL THE EMDING WITH AND GIVE THE UNIQUE DOC NAME FOR THE NAMESCPACE
                await index.Upsert(vectors, chat.Id.ToString());
                return Results.Ok(new ChatResponseDto()
                {
                    Id = chat.Id,
                    ChatId = chat.Id,
                    DocumentId = chat.Document.Id,
                    DocumentName = chat.Document.DocumentName,
                    CreatedOn = chat.CreatedOn.ToString("dd/MM/yyyy"),
                    DocumentUniqueName = chat.Document.DocumentUniqueName

                });

            }).RequireAuthorization();

            // chat with ai 
            routes.MapGet("/api/send-message/{chatId}", async (Guid chatId, HttpContext context, ApplicationDbContext dbContext, AuthUtils authUtils, OpenAI_API.OpenAIAPI api, PineconeClient pinecone) =>
           {
               try
               {
                   context.Response.Headers.Append("Content-Type", "text/event-stream");
                   context.Response.Headers.Append("Cache-Control", "no-cache");
                   context.Response.Headers.Append("Connection", "keep-alive");
                   // Dependency Injection

                   // END Dependency Injection

                   // Check the user 
                   ApplicationUser? user = authUtils.GetLoggedUser(context.Request.HttpContext);
                   if (user is null)
                       return;





                   // check if the chat exist and has the same userId as the logged user
                   Chat? chat = await dbContext.Chats
                   .Where(c => c.Id == chatId).Include(c => c.User)
                   .Include(c => c.Document)
                   .FirstOrDefaultAsync();


                   if (chat is null)
                       return;

                   if (chat.UserId != user.Id)
                       return;

                   if (user.NumberOfQuestions <= 0)
                       return;

                   //retreive the message from the query 
                   var message = context.Request.Query["message"];




                   // get emebdding from the message
                   var embd = await api.Embeddings.GetEmbeddingsAsync(message);

                   var indexName = AppContstants.PINCONE_INDEX_NAME;
                   var index = await pinecone.GetIndex(indexName);
                   //Query the result from pinecone
                   var res = await index.Query(
                            embd, topK: 5, includeMetadata: true, indexNamespace: chat.Id.ToString()
                       );


                   if (!res.Any())
                   {
                       embd = await api.Embeddings.GetEmbeddingsAsync(chat.Document!.DocumentName);
                       res = await index.Query(
                            embd, topK: 5, includeMetadata: true, indexNamespace: chat.Id.ToString()
                       );
                   }


                   var hightScotre = res.Max(e => e.Score);

                   var doc = res.Where(e => e.Score == hightScotre).First();


                   var ctx = @$"AI assistant is a brand new, powerful, human-like artificial intelligence.
    The traits of AI include expert knowledge, helpfulness, cleverness, and articulateness.
    AI is a well-behaved and well-mannered individual.    
    AI is always friendly, kind, and inspiring, and he is eager to provide vivid and thoughtful responses to the user.
    AI has the sum of all knowledge in their brain, and is able to accurately answer nearly any question about any topic in conversation.
      START CONTEXT BLOCK
     {doc.Metadata?["text"]}
      END OF CONTEXT BLOCK    
     AI assistant will take into account any CONTEXT BLOCK that is provided in a conversation.   
     AI assistant will not apologize for previous responses, but instead will indicated new information was gained. 
     AI assistant will not invent anything that is not drawn directly from the context.
        ";

                   var aiChat = api.Chat.CreateConversation();
                   aiChat.AppendSystemMessage(ctx);

                   aiChat.AppendUserInput(message);


                   var aiMessage = "";





                   await aiChat.StreamResponseFromChatbotAsync(async aiChunck =>
                   {

                       aiMessage += aiChunck;
                       await context.Response.WriteAsync($"data: ");
                       var newMessage = new EventChatStreamResponse()
                       {
                           Message = aiChunck
                       };
                       await JsonSerializer.SerializeAsync(context.Response.Body, newMessage);
                       await context.Response.WriteAsync($"\n\n");
                       await context.Response.Body.FlushAsync();
                   });


                   user.NumberOfQuestions -= 1;
                   var newMessage = new Message()
                   {
                       UserId = user.Id,
                       Chat = chat,
                       ChatId = chat.Id,
                       Content = message!,
                       Role = ChatRole.USER.ToString(),
                       CreatedOn = DateTime.UtcNow
                   };

                   chat.Messages.Add(newMessage);
                   await dbContext.SaveChangesAsync();

                   var newAiMessage = new Message()
                   {
                       UserId = user.Id,
                       Chat = chat,
                       ChatId = chat.Id,
                       Content = aiMessage,
                       Role = ChatRole.BOOT.ToString(),
                       CreatedOn = DateTime.UtcNow
                   };

                   chat.Messages.Add(newAiMessage);

                   await dbContext.SaveChangesAsync();
                   context.Response.Body.Close();

               }
               catch (System.Exception e)
               {


               }

           }).RequireAuthorization();


            routes.MapGet("/api/chats/{chatId}", async (Guid chatId, ApplicationDbContext dbContext, HttpContext httpContext, AuthUtils authUtils) =>
            {
                ApplicationUser? user = authUtils.GetLoggedUser(httpContext);
                if (user is null)
                    return Results.Unauthorized();

                GetChatResponseDto? chat = await dbContext.Chats
                .Include(c => c.User)
                .Include(c => c.Document)
                .Include(c => c.Messages)
                .Where(c => c.Id == chatId && c.UserId == user.Id)
                .Select(c => new GetChatResponseDto()
                {

                    ChatId = c.Id,
                    DocumentId = c.Document!.Id,
                    DocumentName = c.Document.DocumentName,
                    DocumentUniqueName = c.Document.DocumentUniqueName,
                    Messages = c.Messages.Select(m =>
                     new MessageResponseDto()
                     {
                         MessageId = m.Id,
                         Content = m.Content,
                         Role = m.Role,


                     }).ToList()

                })
                .FirstOrDefaultAsync();

                if (chat is null)
                    return Results.BadRequest("Cannot find the chat");

                return Results.Ok(chat);

            }).RequireAuthorization();


            return routes;
        }
    }
}